using System.Text.Json.Serialization;
using Sup.Common.Models.Redmine;

namespace Sup.Common.Models.RequestParams;

public class PutIssuesParam 
{
    [JsonPropertyName("issues")]
    public List<RedmineIssue> Issues { get; set; } = [];
}