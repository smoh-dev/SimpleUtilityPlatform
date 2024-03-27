using System.Text.Json;
using Sup.Common.Logger;
using Sup.Np.PageFixer.Models;

namespace Sup.Np.PageFixer.Services;

public class NotionService
{
    private readonly SupLog _log;
    private readonly PageFixerProfiles _profiles;
    
    public NotionService(SupLog log, PageFixerProfiles profiles)
    {
        _log = log.ForContext<NotionService>();
        _profiles = profiles;
    }

    /// <summary>
    /// Test the connection to the Notion API.
    /// </summary>
    /// <returns></returns>
    public async Task<bool> ConnectionTestAsync()
    {
        bool result;
        var requestUrl = $"{_profiles.NotionApiUrl}/users";
        var client = new HttpClient();
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Authorization", "Bearer secret_xMoST8XjEhDftmamqKoKu0qwK2t6iBUlbL84gnsRSJw");
            request.Headers.Add("Notion-Version", _profiles.NotionApiVersion);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            result = true;
        }
        catch (Exception ex)
        {
            _log.Fatal(ex, "{method_name} failed. {error_message}",
                nameof(ConnectionTestAsync), ex.Message);
            result = false;
        }

        return result;
    }

    /// <summary>
    /// Get pages from Notion using issue number(index).
    /// The difference between endIdx and startIdx must be 100.
    /// </summary>
    /// <param name="startIdx"></param>
    /// <param name="endIdx"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<List<NotionPage>> GetIssuePagesAsync(long startIdx, long endIdx)
    {
        using var client = new HttpClient();
        var requestUrl = $"{_profiles.NotionApiUrl}/databases/{_profiles.NotionDbId}/query";
        var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
        request.Headers.Add("Authorization", $"Bearer {_profiles.NotionApiKey}");
        request.Headers.Add("Notion-Version", _profiles.NotionApiVersion);
        var content = new StringContent(GetDatabaseFilter(startIdx, endIdx), null, "application/json");
        request.Content = content;
        try
        {
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var stringContents = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<QueryDatabaseResponse>(stringContents);
            if (result == null)
                throw new NullReferenceException("Deserialized Notion API response is null.");
            _log.Debug("Get pages from notion({current_index} ~ {next_index}): {search_result_count}", 
                startIdx, endIdx, result.NotionPages.Count);
            return result.NotionPages.Where(x => x.Properties.Title.PageTitle.Count > 0).ToList();
        }
        catch (Exception e)
        {
            _log.Error(e, e.Message);
            return [];
        }
    }
    
    /// <summary>
    /// Create filter json for Notion database query.
    /// </summary>
    /// <param name="startIdx"></param>
    /// <param name="endIdx"></param>
    /// <returns></returns>
    private string GetDatabaseFilter(long startIdx, long endIdx)
    {
        string filter = @"{""filter"": {""and"": [" +
            @"{""property"": ""Number"",""number"": {""greater_than_or_equal_to"": " + startIdx + "}}," +
            @"{""property"": ""Number"",""number"": {""less_than"": " + endIdx + "}}]}," + 
            @"""sorts"": [{""property"": ""Number"",""direction"": ""ascending""}]}";
        return filter;
    }
}