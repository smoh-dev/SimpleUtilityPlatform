using System.Data;
using Sup.Common;
using Sup.Common.Configs;
using Sup.Common.Logger;
using Sup.Common.Utils;
using Sup.Np.PagePublisher.Services;

namespace Sup.Np.PagePublisher;

public class PagePublisherWorker : BackgroundService
{
    private readonly int _interval;
    private readonly PagePublisherProfiles _profiles;
    private readonly SupLog _log;
    private readonly ApiService _apiSvc;
    private readonly NotionService _notionSvc;

    public PagePublisherWorker(IConfiguration configs)
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

        // Create loader and api service.
        var encKey = Environment.GetEnvironmentVariable("ENCRYPT_KEY") ?? configs["EncryptKey"];
        if (encKey == null)
            throw new NoNullAllowedException("EncryptKey is not set.");
        var enc = new Encrypter(encKey);
        var esConfigs = new EsConfigs(Consts.ProductCode.NpPagePublisher, apiUrl);
        esConfigs.EsPassword = enc.Decrypt(esConfigs.EsPassword);
        _log = new SupLog(true, esConfigs);
        if (_log.IsEnabledEsLog())
        {
            _log.Verbose("{es_url}({es_index})/{es_user}@{es_password}"
                , esConfigs.EsUrl, esConfigs.EsIndex, esConfigs.EsUser, esConfigs.EsPassword.Length);
            _log.Info("Elasticsearch log enabled.");
        }

        _apiSvc = new ApiService(_log, apiUrl);

        // Load profiles.       
        _profiles = _apiSvc.GetProfilesAsync().Result;

        // Create notion service and test.
        _notionSvc = new NotionService(_log, _profiles);
        if (!_notionSvc.ConnectionTestAsync().Result)
            throw new Exception("Failed to connect to Notion API.");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_interval, stoppingToken);
            if (!CheckSchedule()) continue;
            
            var issuesToPublish = await _apiSvc.GetIssuesToPublishAsync();
            if (issuesToPublish.Count > 0)
            {
                _log.Verbose("Found {issue_count} issues to publish.", issuesToPublish.Count);
                var publishResult = await _notionSvc.PublishPagesAsync(issuesToPublish);
                await _apiSvc.PutPagesAsync(publishResult);
            }
            else
            {
                _log.Verbose("No issues to publish.");
            }
        }
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
}