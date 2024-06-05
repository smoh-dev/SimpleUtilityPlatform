using System.Text.Json.Serialization;
using Sup.Common.Models.DTO;

namespace Sup.Np.IssueLoader;

public class IssueLoaderProfiles
{
    [JsonPropertyName("redmine_url")]
    public string RedmineUrl { get; set; } = string.Empty;
    
    [JsonPropertyName("redmine_api_key")]
    public string RedmineApiKey { get; set; } = string.Empty;
    
    [JsonPropertyName("recover_duration")]
    public int RecoverDuration { get; set; } = 0;
    
    [JsonPropertyName("target_project_ids")]
    public List<long> TargetProjectIds { get; set; } = new();

    public ScheduleDto Schedule { get; set; } = new();
}