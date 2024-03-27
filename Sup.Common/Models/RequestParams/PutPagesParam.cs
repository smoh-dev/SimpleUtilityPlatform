using System.Text.Json.Serialization;

namespace Sup.Common.Models.RequestParams;

public class PutPageParam
{
    [JsonPropertyName("issue_number")]
    public long IssueNumber { get; set; }
    
    [JsonPropertyName("page_id")]
    public string PageId { get; set; } = String.Empty;
    
    [JsonPropertyName("posted_at")]
    public DateTime PostedAt { get; set;}
}

public class PutPagesParam
{
    [JsonPropertyName("pages")]
    public List<PutPageParam> Pages { get; set; } = [];
}
