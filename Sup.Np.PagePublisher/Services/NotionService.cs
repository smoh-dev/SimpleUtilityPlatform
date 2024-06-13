using System.Data;
using System.Text.Json;
using Sup.Common.Kms;
using Sup.Common.Logger;
using Sup.Common.Models.Notion;
using Sup.Common.Models.RequestParams;
using Sup.Common.Models.Responses;

namespace Sup.Np.PagePublisher.Services;

public class NotionService
{
    private readonly SupLog _log;
    private readonly string _redmineApiUrl;
    private readonly string _notionApiUrl;
    private readonly string _notionApiVersion;
    private readonly string _notionDbId;
    private readonly string _notionApiKey;

    public NotionService(SupLog log, PagePublisherProfiles profiles, AwsKmsEncryptor awsKmsEncryptor)
    {
        _log = log.ForContext<NotionService>();
        _redmineApiUrl = profiles.RedmineUrl;
        _notionApiUrl = profiles.NotionApiUrl;
        _notionApiVersion = profiles.NotionApiVersion;
        _notionDbId = profiles.NotionDbId;
        _notionApiKey = awsKmsEncryptor.DecryptStringAsync(profiles.NotionApiKey).Result;
    }

    /// <summary>
    /// Test the connection to the Notion API.
    /// </summary>
    /// <returns></returns>
    public async Task<bool> ConnectionTestAsync()
    {
        bool result;
        var requestUrl = $"{_notionApiUrl}/users";
        var client = new HttpClient();
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Authorization", "Bearer " + _notionApiKey);
            request.Headers.Add("Notion-Version", _notionApiVersion);
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

    public async Task<PutPagesParam> PublishPagesAsync(List<IssueToPublish> issues)
    {
        var pagesToPost = new List<NotionPage>();
        var pagesToPatch = new List<NotionPage>();

        try
        {
            var pages = issues.Select(i =>
                new NotionPage(_notionDbId, i.PageId, i.IssueNumber, i.Title, i.Status, i.Type,
                    $"{_redmineApiUrl}/issues/{i.IssueNumber}", i.Author, i.AssignedTo)
            ).ToList();

            pagesToPost = pages.Where(p => string.IsNullOrEmpty(p.ExistingPageId)).ToList();
            pagesToPatch = pages.Where(p => !string.IsNullOrEmpty(p.ExistingPageId)).ToList();

        }
        catch (Exception ex)
        {
            _log.Fatal(ex, "{method_name} failed. {error_message}",
                nameof(PublishPagesAsync), ex.Message);
        }
        
        var result = new PutPagesParam();
        result.Pages.AddRange(await PostPageAsync(pagesToPost));
        result.Pages.AddRange(await PatchPageAsync(pagesToPatch));
        return result;
    }

    /// <summary>
    /// Update existing notion pages.
    /// </summary>
    /// <param name="pages"></param>
    private async Task<List<PutPageParam>> PatchPageAsync(List<NotionPage> pages)
    {
        var result = new List<PutPageParam>();
        foreach (var page in pages)
        {
            var requestUrl = $"{_notionApiUrl}/pages/{page.ExistingPageId}";
            var request = new HttpRequestMessage(HttpMethod.Patch, requestUrl);
            request.Headers.Add("Notion-Version", _notionApiVersion);
            request.Headers.Add("Authorization", "Bearer " + _notionApiKey);
            try
            {
                var json = JsonSerializer.Serialize(page);
                var content = new StringContent(json, null, "application/json");
                request.Content = content;
                using var client = new HttpClient();
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var postPageResponse = JsonSerializer.Deserialize<PostPageResponse>(await response.Content.ReadAsStringAsync());
                if (postPageResponse == null)
                    throw new NoNullAllowedException("Deserialization failed.");

                result.Add(new PutPageParam
                {
                    PageId = postPageResponse.PostedPageId,
                    IssueNumber = page.Properties.Number.Number,
                    PostedAt = DateTime.Now
                });
                
                _log.Debug("{method_name} success. Issue {issue_id} has been updated to pages {notion_page_id}.",
                    nameof(PatchPageAsync), page.Properties.Number.Number, postPageResponse.PostedPageId);
                
                await Task.Delay(500); // To avoid API rate limit exceeded.
            }
            catch (Exception ex)
            {
                _log.Fatal(ex, "{method_name} failed({issue_number}).  {error_message}",
                    nameof(PatchPageAsync), page.Properties.Number.Number, ex.Message);
            }
        }

        return result;
    }

    /// <summary>
    /// Create new notion pages.
    /// </summary>
    /// <param name="pages"></param>
    /// <exception cref="NoNullAllowedException"></exception>
    private async Task<List<PutPageParam>> PostPageAsync(List<NotionPage> pages)
    {
        var result = new List<PutPageParam>();
        var requestUrl = $"{_notionApiUrl}/pages";

        foreach (var page in pages)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
                request.Headers.Add("Notion-Version", _notionApiVersion);
                request.Headers.Add("Authorization", "Bearer " + _notionApiKey);
                var json = JsonSerializer.Serialize(page);
                var content = new StringContent(json, null, "application/json");
                request.Content = content;
                using var client = new HttpClient();
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var postPageResponse = JsonSerializer.Deserialize<PostPageResponse>(await response.Content.ReadAsStringAsync());
                if (postPageResponse == null)
                    throw new NoNullAllowedException("Deserialization failed.");

                result.Add(new PutPageParam
                {
                    PageId = postPageResponse.PostedPageId,
                    IssueNumber = page.Properties.Number.Number,
                    PostedAt = DateTime.Now
                });
                
                _log.Debug("{method_name} success. Issue {issue_id} has been posted to pages {notion_page_id}.",
                    nameof(PostPageAsync), page.Properties.Number.Number, postPageResponse.PostedPageId);
                
                await Task.Delay(500); // To avoid API rate limit exceeded.
            }
            catch (Exception ex)
            {
                _log.Fatal(ex, "{method_name} failed({issue_number}).  {error_message}",
                    nameof(PostPageAsync), page.Properties.Number.Number, ex.Message);
            }
        }

        return result;
    }
}