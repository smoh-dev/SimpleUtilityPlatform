using System.Data;
using System.Net;
using System.Text.Json;
using Sup.Common;
using Sup.Common.Logger;
using Sup.Common.Models.RequestParams;
using Sup.Common.Models.Responses;

namespace Sup.Np.PagePublisher.Services;

public class ApiService
{
    private readonly SupLog _log;
    private readonly string _url;
    public ApiService(SupLog log, string apiUrl)
    {
        _log = log.ForContext<ApiService>();
        _url = apiUrl;
    }
    
    /// <summary>
    /// Get the profiles for PagePublisher.
    /// </summary>
    /// <returns></returns>
    public async Task<PagePublisherProfiles> GetProfilesAsync()
    {
        PagePublisherProfiles result = new();
        
        using var client = new HttpClient();
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
                    case Consts.ProfileEntries.NotionApiUrl:
                        result.NotionApiUrl = profile.Value;
                        break;
                    case Consts.ProfileEntries.NotionApiVersion:
                        result.NotionApiVersion = profile.Value;
                        break;
                    case Consts.ProfileEntries.NotionDbId:
                        result.NotionDbId = profile.Value;
                        break;
                    case Consts.ProfileEntries.NotionApiKey:
                        result.NotionApiKey = profile.Value;
                        break;
                    case Consts.ProfileEntries.RedmineUrl:
                        result.RedmineUrl = profile.Value;
                        break;
                }
            }
            
            _log.Debug("{method_name} success.",
                nameof(GetProfilesAsync));
        }
        catch (Exception ex)
        {
            _log.Fatal(ex, "{method_name} failed. {error_message}",
                nameof(GetProfilesAsync), ex.Message);
        }

        return result;
    }
    
    /// <summary>
    /// Get issues that require POST or FETCH pages.
    /// </summary>
    /// <returns></returns>
    public async Task<List<IssueToPublish>> GetIssuesToPublishAsync()
    {
        var issuesToPublish = new List<IssueToPublish>();
        try
        {
            using var client = new HttpClient();
            var requestUrl = $"{_url}/PagePublisher/issues";
            var response = await client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();
            if (response.StatusCode == HttpStatusCode.NoContent)
                return issuesToPublish;
            var responseString = await response.Content.ReadAsStringAsync();
            var issuesToUpdateResponse = JsonSerializer.Deserialize<GetIssuesToPublishResponse>(responseString);
            if (issuesToUpdateResponse != null)
                issuesToPublish =  issuesToUpdateResponse.IssuesToUpdate;
        }
        catch (Exception e)
        {
            _log.Error(e, "Failed to get issues to update.");
        }

        return issuesToPublish;
    }

    /// <summary>
    /// Saves the results of pages POST or PATCH to Notion.
    /// </summary>
    /// <param name="param"></param>
    public async Task PutPagesAsync(PutPagesParam param)
    {
        if (param.Pages.Count == 0)
            return;
        
        using var client = new HttpClient();
        var requestUrl = $"{_url}/PagePublisher/pages";
        var request = new HttpRequestMessage(HttpMethod.Put, requestUrl);
        try
        {
            var json = JsonSerializer.Serialize(param);
            var content = new StringContent(json, null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request); 
            response.EnsureSuccessStatusCode();
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                _log.Warn("{method_name} success but no pages affected. {issue_numbers}",
                    nameof(PutPagesAsync), string.Join(',', param.Pages.Select(page => page.IssueNumber).ToList()));
                return;
            }
            var result = JsonSerializer.Deserialize<ModifyResultResponse>(await response.Content.ReadAsStringAsync());
            if (result == null)
                throw new NoNullAllowedException("Deserialization failed.");
            
            _log.Debug("{method_name} success. {total_count} pages out of {affected_row_count} pages processed.",
                nameof(PutPagesAsync), param.Pages.Count, result.AffectedRowCount);
        }
        catch (Exception ex)
        {
            _log.Fatal(ex, "{method_name} failed. {error_message}",
                nameof(PutPagesAsync), ex.Message);
        }    
    }
}