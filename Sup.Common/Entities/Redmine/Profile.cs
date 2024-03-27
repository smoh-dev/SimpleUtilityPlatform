using System.Text.Json.Serialization;

namespace Sup.Common.Entities.Redmine;

public class Profile
{
    [JsonPropertyName("entiry")]
    public string Entry { get; set; } = string.Empty;
    
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}