using System.Text.Json.Serialization;

namespace Sup.Common.Entities.Redmine;

public class License
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;
    [JsonPropertyName("product")]
    public string Product { get; set; } = string.Empty;
    [JsonPropertyName("auth_audience")]
    public string AuthAudience { get; set; } = string.Empty;
    [JsonPropertyName("signing_key")]
    public string AuthSigningKey { get; set; } = string.Empty;
}