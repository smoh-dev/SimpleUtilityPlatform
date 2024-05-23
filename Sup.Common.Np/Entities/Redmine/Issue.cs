using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Sup.Common.Entities.Redmine;

public class Issue
{
    [Column("id")]
    [JsonPropertyName("id")]
    public long Id { get; set; }
    
    [Column("project_id")]
    [JsonPropertyName("project_id")]
    public long ProjectId { get; set; }
    
    [Column("type")]
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [Column("status")]
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [Column("priority")]
    [JsonPropertyName("priority")]
    public string Priority { get; set; } = string.Empty;
    
    [Column("assigned_to")]
    [JsonPropertyName("assigned_to")]
    public string AssignedTo { get; set; } = string.Empty;
    
    [Column("target_version")]
    [JsonPropertyName("target_version")]
    public string TargetVersion { get; set; } = string.Empty;
    
    [Column("category_name")]
    [JsonPropertyName("category_name")]
    public string CategoryName { get; set; } = string.Empty;
    
    [Column("parent_issue_id")]
    [JsonPropertyName("parent_issue_id")]
    public long ParentIssueId { get; set; }
    
    [Column("title")]
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    [Column("created_on")]
    [JsonPropertyName("created_on")]
    public DateTime CreatedOn { get; set; }
    
    [Column("updated_on")]
    [JsonPropertyName("updated_on")]
    public DateTime UpdatedOn { get; set; }

    [Column("last_posted_on")]
    [JsonPropertyName("last_posted_on")]
    public DateTime? LastPostedOn { get; set; } = null;
    
    [Column("author")]
    [JsonPropertyName("author")]
    public string Author { get; set; } = string.Empty;

    public Issue()
    {
        
    }

    public Issue(Issue issue)
    {
        Id = issue.Id;
        ProjectId = issue.ProjectId;
        Type = issue.Type;
        Status = issue.Status;
        Priority = issue.Priority;
        AssignedTo = issue.AssignedTo;
        TargetVersion = issue.TargetVersion;
        CategoryName = issue.CategoryName;
        ParentIssueId = issue.ParentIssueId;
        Title = issue.Title;
        CreatedOn = issue.CreatedOn;
        UpdatedOn = issue.UpdatedOn;
        LastPostedOn = issue.LastPostedOn;
        Author = issue.Author;
    }
}