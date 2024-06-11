using System.Data;
using System.Text;
using System.Text.Json;
using Sup.Common;
using Sup.Common.Configs;
using Sup.Common.Logger;
using Sup.Common.Models.Redmine;
using Sup.Common.Models.RequestParams;
using Sup.Common.TokenManager;
using Sup.Common.Utils;
using Sup.Np.IssueLoader.Services;

namespace Sup.Np.IssueLoader;

public class IssueLoaderWorker : BackgroundService
{
    private readonly int _interval;
    private readonly RedmineService _redmineSvc;
    private readonly ApiService _apiSvc;
    private readonly IssueLoaderProfiles _profiles;
    private readonly TokenManager _tokenManager;

    public IssueLoaderWorker(IConfiguration configs, TokenManager tokenManager)
    {
#if DEBUG
        _interval = 1000 * 5;
#else
        _interval = 1000 * 60;
#endif

        // Validate.
        var apiHost = Environment.GetEnvironmentVariable("API_HOST");
        var apiPort = Environment.GetEnvironmentVariable("API_PORT");
        var apiUrl = !string.IsNullOrEmpty(apiHost) ? $"http://{apiHost}:{apiPort}" : configs["ApiUrl"];
        if (string.IsNullOrEmpty(apiUrl))
            throw new NoNullAllowedException("Api url is not set.");

        // Check license.
        var licenseKey = Environment.GetEnvironmentVariable("LICENSE_KEY") ?? configs["LicenseKey"];
        if (licenseKey == null)
            throw new NoNullAllowedException("LicenseKey is not set.");
        if (!CheckLicenseAsync(apiUrl, licenseKey).Result)
            throw new HttpRequestException("Failed to check license.");

        // Create loader and api service.
        var encKey = Environment.GetEnvironmentVariable("ENCRYPT_KEY") ?? configs["EncryptKey"];
        if (encKey == null)
            throw new NoNullAllowedException("EncryptKey is not set.");
        var enc = new Encrypter(encKey);
        var esConfigs = new EsConfigs(Consts.ProductCode.NpIssueLoader, apiUrl);
        esConfigs.EsPassword = enc.Decrypt(esConfigs.EsPassword);
        var log = new SupLog(true, esConfigs);
        if (log.IsEnabledEsLog())
        {
            log.Verbose("{es_url}({es_index})/{es_user}@{es_password}"
                , esConfigs.EsUrl, esConfigs.EsIndex, esConfigs.EsUser, esConfigs.EsPassword.Length);
            log.Info("Elasticsearch log enabled.");
        }

        _apiSvc = new ApiService(log, tokenManager, apiUrl);

        // Load profiles.       
        _profiles = _apiSvc.GetProfilesAsync().Result;

        // Create redmine service.
        _redmineSvc = new RedmineService(log, _profiles);

        log = log.ForContext<IssueLoaderWorker>();
        log.Info("{class_name} created.", nameof(IssueLoaderWorker));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // When the IssueLoader starts, it receives projects list from Redmine and updates the DB.
        var redmineProjects = await _redmineSvc.GetProjectsAsync();
        await _apiSvc.PutProjectsAsync(redmineProjects);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_interval, stoppingToken);
            if (!CheckSchedule()) continue;

            var unpublishedIssueNumbers = await _apiSvc.GetUnpublishedIssuesAsync();
            List<RedmineIssue> redmineIssues = new();
            if (unpublishedIssueNumbers.Count > 0)
                redmineIssues = await GetUnpublishedIssues(unpublishedIssueNumbers);

            foreach (var id in _profiles.TargetProjectIds)
                redmineIssues.AddRange(await _redmineSvc.GetIssuesAsync(id));

            if (redmineIssues.Count > 0)
                await _apiSvc.PutIssuesAsync(redmineIssues);
        }
    }

    /// <summary>
    /// Get issues by issue ids from Redmine.
    /// </summary>
    /// <param name="issueNumbers"></param>
    /// <returns></returns>
    private async Task<List<RedmineIssue>> GetUnpublishedIssues(List<long> issueNumbers)
    {
        var result = new List<RedmineIssue>();
        var offset = 0;
        var issueNumberArg = "";
        foreach (var issueNumber in issueNumbers)
        {
            if (offset == 0) issueNumberArg = "";
            issueNumberArg += $"{issueNumber},";
            offset++;
            if (offset != 100) continue;
            result.AddRange(await _redmineSvc.GetIssuesAsync(issueNumberArg.TrimEnd(',')));
            offset = 0;
        }

        result.AddRange(await _redmineSvc.GetIssuesAsync(issueNumberArg.TrimEnd(',')));

        // Deleted issues
        var retrievedIssueNumbers = result.Select(issue => issue.Id).ToList();
        var deletedIssueNumbers = issueNumbers.Except(retrievedIssueNumbers).ToList();
        var deletedIssues = deletedIssueNumbers.Select(deletedIssueNumber => new RedmineIssue
            {
                Id = deletedIssueNumber,
                Status = new RedmineFields { Name = "UnManaged" },
                Title = "Deleted Issues",
            })
            .ToList();
        await _apiSvc.PutIssuesAsync(deletedIssues);

        return result;
    }

    private bool CheckSchedule()
    {
        var isScheduled = true;
        var now = DateTime.Now;
        int currentHour = now.TimeOfDay.Hours, startHour = 6, endHour = 17;
        switch (now.DayOfWeek)
        {
            case DayOfWeek.Sunday:
                isScheduled = _profiles.Schedule.WorkingInSunday;
                startHour = _profiles.Schedule.SundayStartHour;
                endHour = _profiles.Schedule.SundayEndHour;
                break;
            case DayOfWeek.Monday:
                isScheduled = _profiles.Schedule.WorkingInMonday;
                startHour = _profiles.Schedule.MondayStartHour;
                endHour = _profiles.Schedule.MondayEndHour;
                break;
            case DayOfWeek.Tuesday:
                isScheduled = _profiles.Schedule.WorkingInTuesday;
                startHour = _profiles.Schedule.TuesdayStartHour;
                endHour = _profiles.Schedule.TuesdayEndHour;
                break;
            case DayOfWeek.Wednesday:
                isScheduled = _profiles.Schedule.WorkingInWednesday;
                startHour = _profiles.Schedule.WednesdayStartHour;
                endHour = _profiles.Schedule.WednesdayEndHour;
                break;
            case DayOfWeek.Thursday:
                isScheduled = _profiles.Schedule.WorkingInThursday;
                startHour = _profiles.Schedule.ThursdayStartHour;
                endHour = _profiles.Schedule.ThursdayEndHour;
                break;
            case DayOfWeek.Friday:
                isScheduled = _profiles.Schedule.WorkingInFriday;
                startHour = _profiles.Schedule.FridayStartHour;
                endHour = _profiles.Schedule.FridayEndHour;
                break;
            case DayOfWeek.Saturday:
                isScheduled = _profiles.Schedule.WorkingInFriday;
                startHour = _profiles.Schedule.FridayStartHour;
                endHour = _profiles.Schedule.FridayEndHour;
                break;
        }

        return isScheduled && currentHour >= startHour && currentHour < endHour;
    }

    /// <summary>
    /// Check license before start the IssueLoader.
    /// </summary>
    /// <param name="apiUrl"></param>
    /// <param name="licenseKey"></param>
    /// <exception cref="HttpRequestException">Failed to check license</exception>
    /// <returns></returns>
    private async Task<bool> CheckLicenseAsync(string apiUrl, string licenseKey)
    {
        using var client = new HttpClient();
        var requestUrl = $"{apiUrl}/Common/license";
        var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
        var param = new CheckLicenseParam
        {
            ProductCode = Consts.ProductCode.NpIssueLoader,
            LicenseKey = licenseKey,
        };
        var json = JsonSerializer.Serialize(param);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return true;
    }
}