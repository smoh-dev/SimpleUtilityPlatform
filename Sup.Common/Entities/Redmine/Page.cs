using System.Text.Json.Serialization;

namespace Sup.Common.Entities.Redmine;

public class Page
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("issue_id")]
    public long IssueId { get; set; }
    
    [JsonPropertyName("posted_at")]
    public DateTime PostedAt { get; set; }
    
    [JsonPropertyName("account_id")]
    public long AccountId { get; set; }
    
}
