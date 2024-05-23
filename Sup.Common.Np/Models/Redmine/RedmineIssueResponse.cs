using System.Text.Json.Serialization;

namespace Sup.Common.Models.Redmine;

public class RedmineIssueResponse
{
    [JsonPropertyName("issue")]
    public RedmineIssue Issue { get; set; } = new();
}