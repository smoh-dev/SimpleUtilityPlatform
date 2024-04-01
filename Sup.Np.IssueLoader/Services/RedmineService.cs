using System.Text.Json;
using Sup.Common.Logger;
using Sup.Common.Models.Redmine;

namespace Sup.Np.IssueLoader.Services;

public class RedmineService(SupLog log, IssueLoaderProfiles profiles)
{
    private const int Limit = 100;
    private readonly SupLog _log = log.ForContext<RedmineService>();
    private readonly string _url = profiles.RedmineUrl;
    private readonly string _key = profiles.RedmineApiKey;
    private DateTime _lastLoadedTime = DateTime.Now - TimeSpan.FromDays(profiles.RecoverDuration);

    /// <summary>
    /// Get all projects from Redmine.
    /// </summary>
    /// <returns></returns>
    public async Task<List<RedmineProject>> GetProjectsAsync()
    {
        var result = new List<RedmineProject>();
        var baseUrl = $"{_url}/projects.json";
        int offset = 0, totalCount = 0;

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("X-Redmine-API-Key", _key);
        do
        {
            try
            {
                var requestUrl = $"{baseUrl}?offset={offset}&limit={Limit}";
                var response = await client.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var redmineProjectsResponse = JsonSerializer.Deserialize<RedmineProjectsResponse>(content);
                if (redmineProjectsResponse == null) break;
                totalCount = redmineProjectsResponse.TotalCount;
                result.AddRange(redmineProjectsResponse.Projects);

                await Task.Delay(500); // To avoid API rate limit exceeded.
            }
            catch (Exception ex)
            {
                _log.Fatal(ex, "{method_name} failed. {error_message}",
                    nameof(GetProjectsAsync), ex.Message);
            }
            finally
            {
                offset += Limit;
            }
        } while (offset < totalCount);

        _log.Debug("{method_name} success. {result_count} projects loaded.",
            nameof(GetProjectsAsync), result.Count);
        return result;
    }

    /// <summary>
    /// Get issue list by projectId from Redmine.
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<List<RedmineIssue>> GetIssuesAsync(long projectId)
    {
        List<RedmineIssue> issues = [];
        var baseUrl = $"{_url}/issues.json?project_id={projectId}";
        int offset = 0, totalCount = 0;

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("X-Redmine-API-Key", _key);
        do
        {
            try
            {
                var requestUrl = $"{baseUrl}&status_id=*&offset={offset}&limit={Limit}" +
                                 $"&updated_on=%3E%3D{_lastLoadedTime:yyyy-MM-ddTHH:mm:ssZ}";
                var response = await client.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var contents = await response.Content.ReadAsStringAsync();
                var redmineIssuesResponse = JsonSerializer.Deserialize<RedmineIssuesResponse>(contents);
                if (redmineIssuesResponse == null) break;
                totalCount = redmineIssuesResponse.TotalCount;
                issues.AddRange(redmineIssuesResponse.Issues);

                await Task.Delay(500); // To avoid API rate limit exceeded.
            }
            catch (Exception ex)
            {
                _log.Fatal(ex, "{method_name} failed. {error_message}",
                    nameof(GetProjectsAsync), ex.Message);
            }
            finally
            {
                offset += Limit;
            }
        } while (offset < totalCount);
        _log.Verbose("{method_name} details: {project_id}({updated_on})"
            , nameof(GetIssuesAsync), projectId, _lastLoadedTime);
        _lastLoadedTime = DateTime.Now;

        _log.Debug("{method_name} get {issue_count} issues from project {projectId}.",
            nameof(GetIssuesAsync), issues.Count, projectId);

        return issues;
    }


    public async Task<List<RedmineIssue>> GetIssuesAsync(string issueIds)
    {
        List<RedmineIssue> issues = [];
        var baseUrl = $"{_url}/issues.json?issue_id={issueIds}";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("X-Redmine-API-Key", _key);
        try
        {
            var response = await client.GetAsync($"{baseUrl}&status_id=*&offset=0&limit={Limit}");
            response.EnsureSuccessStatusCode();

            var contents = await response.Content.ReadAsStringAsync();
            var redmineIssuesResponse = JsonSerializer.Deserialize<RedmineIssuesResponse>(contents);
            if(redmineIssuesResponse != null)
                issues.AddRange(redmineIssuesResponse.Issues);

            await Task.Delay(500); // To avoid API rate limit exceeded.
        }
        catch (Exception ex)
        {
            _log.Fatal(ex, "{method_name} failed. {error_message}",
                nameof(GetProjectsAsync), ex.Message);
        }
            
        _log.Verbose("{method_name} details: {issue_count}"
            , nameof(GetIssuesAsync), issues.Count, _lastLoadedTime);
        _lastLoadedTime = DateTime.Now;

        _log.Debug("{method_name} get {issue_count} issues.",
            nameof(GetIssuesAsync), issues.Count);

        return issues;
    }
}