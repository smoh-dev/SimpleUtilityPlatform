using System.Text.Json.Serialization;

namespace Sup.Common.Entities.Redmine;

public class License
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;
    [JsonPropertyName("product")]
    public string Product { get; set; } = string.Empty;
}