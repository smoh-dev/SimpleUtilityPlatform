using System.Text.Json.Serialization;

namespace Sup.Common.Models.Responses;

public class IssueToPublish
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("author")]
    public string Author { get; set; } = string.Empty;
    
    [JsonPropertyName("issue_number")]
    public long IssueNumber { get; set; }
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("assigned_to")]
    public string AssignedTo { get; set; } = string.Empty;
    
    [JsonPropertyName("page_id")]
    public string PageId { get; set; } = string.Empty;
}

public class GetIssuesToPublishResponse : ApiResponse
{
    public List<IssueToPublish> IssuesToUpdate { get; set; } = [];

    public GetIssuesToPublishResponse()
    {
        
    }

    public GetIssuesToPublishResponse(bool success, int errorCode, string message) : base(success, errorCode, message)
    {
        
    }
}