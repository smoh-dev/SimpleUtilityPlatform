using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Sup.Common.Entities.Redmine;
public class Project
{
    [Column("id")]
    [JsonPropertyName("id")]
    public long Id { get; set; }
    
    [Column("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
