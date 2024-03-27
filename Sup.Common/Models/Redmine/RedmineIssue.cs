using System.Text.Json.Serialization;

namespace Sup.Common.Models.Redmine;

public class RedmineFields
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}

public class RedmineIssue
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("project")]
    public RedmineFields Project { get; set; } = new();
    
    [JsonPropertyName("tracker")]
    public RedmineFields Type { get; set; } = new();
    
    [JsonPropertyName("status")]
    public RedmineFields Status { get; set; } = new();
    
    [JsonPropertyName("priority")]
    public RedmineFields Priority { get; set; } = new();
    
    [JsonPropertyName("author")]
    public RedmineFields Author { get; set; } = new();
    
    [JsonPropertyName("assigned_to")]
    public RedmineFields AssignedTo { get; set; } = new();  
    
    [JsonPropertyName("category")]
    public RedmineFields Category { get; set; } = new();
    
    [JsonPropertyName("fixed_version")]
    public RedmineFields TargetVersion { get; set; } = new();
    
    [JsonPropertyName("parent")]
    public RedmineFields ParentIssue { get; set; } = new();
    
    [JsonPropertyName("subject")]
    public string Title { get; set; } = string.Empty;
    
    [JsonPropertyName("created_on")]
    public DateTime CreatedOn { get; set; }
    
    [JsonPropertyName("updated_on")]
    public DateTime UpdatedOn { get; set; }
}
