using System.Text.Json.Serialization;
using Sup.Common.Models.Redmine;

namespace Sup.Common.Models.RequestParams;

public class PutProjectsParam
{
    [JsonPropertyName("projects")]
    public List<RedmineProject> Projects { get; set; } = [];
}