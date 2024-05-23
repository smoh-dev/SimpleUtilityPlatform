using System.Text.Json.Serialization;

namespace Sup.Common.Models.Redmine;

public class RedmineProjectsResponse
{
    [JsonPropertyName("projects")]
    public RedmineProject[] Projects { get; set; } = [];
    
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
    
    [JsonPropertyName("offset")]
    public int Offset { get; set; }
    
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}