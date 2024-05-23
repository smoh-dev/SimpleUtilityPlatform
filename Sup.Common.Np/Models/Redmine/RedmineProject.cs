using System.Text.Json.Serialization;

namespace Sup.Common.Models.Redmine;
public class RedmineProject
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
