using System.Data;
using System.Net;
using System.Text;
using System.Text.Json;
using Sup.Common;
using Sup.Common.Logger;
using Sup.Common.Models.Redmine;
using Sup.Common.Models.RequestParams;
using Sup.Common.Models.Responses;
using Sup.Common.TokenManager;

namespace Sup.Np.IssueLoader.Services;

public class ApiService(SupLog log, TokenManager tokenManager, string apiUrl )
{
    private readonly SupLog _log = log.ForContext<ApiService>();
    private readonly TokenManager _tokenManager = tokenManager;
    private readonly string _url = apiUrl;

    /// <summary>
    /// Get the profiles for IssueLoader.
    /// </summary>
    /// <returns></returns>
    public async Task<IssueLoaderProfiles> GetProfilesAsync()
    {
        IssueLoaderProfiles result = new();
        
        using var client = await _tokenManager.GetHttpClientAsync();
        var requestUrl = $"{_url}/common/profiles";

        try
        {
            var response = await client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var res = JsonSerializer.Deserialize<GetProfilesResponse>(json);
            if (res == null)
                throw new NoNullAllowedException("Deserialization failed.");

            foreach (var profile in res.Profiles)
            {
                switch (profile.Entry)
                {
                    case Consts.ProfileEntries.RedmineUrl:
                        result.RedmineUrl = profile.Value;
                        break;
                    case Consts.ProfileEntries.RedmineApiKey:
                        result.RedmineApiKey = profile.Value;
                        break;
                    case Consts.ProfileEntries.LoaderRecoverDuration:
                        result.RecoverDuration = int.Parse(profile.Value);
                        break;
                    case Consts.ProfileEntries.LoaderTargetProjectIds:
                        result.TargetProjectIds = profile.Value.Split(',').Select(long.Parse).ToList();
                        break;
                }
            }
            
            _log.Debug("{method_name} success.",
                nameof(GetProfilesAsync));
        }
        catch (Exception ex)
        {
            _log.Fatal(ex, "{method_name} failed. {error_message}({request_url})",
                nameof(GetProfilesAsync), ex.Message, requestUrl);
        }

        return result;
    }

    /// <summary>
    /// Calls the API to update or insert project information in the DB.
    /// </summary>
    /// <param name="projects"></param>
    public async Task PutProjectsAsync(List<RedmineProject> projects)
    {
        using var client = await _tokenManager.GetHttpClientAsync();
        var requestUrl = $"{_url}/IssueLoader/projects";
        var param = new PutProjectsParam { Projects = projects };

        try
        {
            var json = JsonSerializer.Serialize(param);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(requestUrl, content);
            response.EnsureSuccessStatusCode();
            _log.Debug("{method_name} success.",nameof(PutProjectsAsync));
        }
        catch (Exception ex)
        {
            _log.Fatal(ex, "{method_name} failed. {error_message}({request_url})",
                nameof(PutProjectsAsync), ex.Message, requestUrl);
        }
    }
    
    /// <summary>
    /// Calls the API to update or insert issue information in the DB.
    /// </summary>
    /// <param name="issues"></param>
    public async Task PutIssuesAsync(List<RedmineIssue> issues)
    {
        using var client = await _tokenManager.GetHttpClientAsync();
        var requestUrl = $"{_url}/IssueLoader/issues";
        var param = new PutIssuesParam { Issues = issues };

        try
        {
            var json = JsonSerializer.Serialize(param);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(requestUrl, content);
            response.EnsureSuccessStatusCode();
            _log.Debug("{method_name} success.", nameof(PutIssuesAsync));
        }
        catch (Exception ex)
        {
            _log.Fatal(ex, "{method_name} failed. {error_message}({request_url})",
                nameof(PutProjectsAsync), ex.Message, requestUrl);
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<List<long>> GetUnpublishedIssuesAsync()
    {
        using var client = await _tokenManager.GetHttpClientAsync();
        var requestUrl = $"{_url}/IssueLoader/issues/unpublished";
        
        try
        {
            var response = await client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();
            if (response.StatusCode == HttpStatusCode.NoContent) return new List<long>();
            var json = await response.Content.ReadAsStringAsync();
            var res = JsonSerializer.Deserialize<GetUnpublishedIssuesResponse>(json);
            if (res == null)
                throw new NoNullAllowedException("Deserialization failed.");
            
            _log.Debug("{method_name} success.",
                nameof(GetUnpublishedIssuesAsync));
            return res.IssueNumbers;
        }
        catch (Exception ex)
        {
            _log.Fatal(ex, "{method_name} failed. {error_message}({request_url})",
                nameof(GetUnpublishedIssuesAsync), ex.Message, requestUrl);
            return new List<long>();
        }
    }
}