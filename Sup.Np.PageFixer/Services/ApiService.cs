using System.Data;
using System.Text.Json;
using Sup.Common;
using Sup.Common.Logger;
using Sup.Common.Models.RequestParams;
using Sup.Common.Models.Responses;
using Sup.Np.PageFixer.Models;

namespace Sup.Np.PageFixer.Services;

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
    /// Get the profiles for PageFixer.
    /// </summary>
    /// <returns></returns>
    public async Task<PageFixerProfiles> GetProfilesAsync()
    {
        PageFixerProfiles result = new();
        
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
                    case Consts.ProfileEntries.FixerMaxIssueLimit:
                        result.MaxIssueLimit = long.Parse(profile.Value);
                        break;
                    case Consts.ProfileEntries.FixerMinIssueLimit:
                        result.MinIssueLimit = long.Parse(profile.Value);
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
    /// Set page info to DB.
    /// </summary>
    /// <param name="notionPages"></param>
    /// <returns></returns>
    public async Task<bool> PutPagesAsync(List<NotionPage> notionPages)
    {
        PutPagesParam pagesParam = new();
        pagesParam.Pages.AddRange(notionPages.Select(np => new PutPageParam
        {
            IssueNumber = np.Properties.Number.IssueNumber,
            PageId = np.Id,
            PostedAt = np.CreatedTime // Register the page creation date to trigger a refresh of PagePublisher.
        }).ToList());
        
        using var client = new HttpClient();
        var requestUrl = $"{_url}/PagePublisher/pages";
        var request = new HttpRequestMessage(HttpMethod.Put, requestUrl);
        try
        {
            var json = JsonSerializer.Serialize(pagesParam);
            var content = new StringContent(json, null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request); 
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<ModifyResultResponse>(await response.Content.ReadAsStringAsync());
            if (result == null)
                throw new NoNullAllowedException("Deserialization failed.");
            
            _log.Debug("{method_name} success. {total_count} pages out of {affected_row_count} pages processed.",
                nameof(PutPagesAsync), notionPages.Count, result.AffectedRowCount);
            return true;
        }
        catch (Exception ex)
        {
            _log.Fatal(ex, "{method_name} failed({issue_numbers}). {error_message}",
                nameof(PutPagesAsync), string.Join(",",pagesParam.Pages.Select(p=>p.IssueNumber).ToList()) , ex.Message);
            return false;
        }
    }
}