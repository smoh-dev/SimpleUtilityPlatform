using System.Text.Json.Serialization;

namespace Sup.Np.PageFixer.Models;

public class QueryDatabaseResponse(List<NotionPage> notionPages)
{
    [JsonPropertyName("results")]
    public List<NotionPage> NotionPages { get; init; } = notionPages;
}

public class NotionPage
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("created_time")]
    public DateTime CreatedTime { get; set; }
    
    [JsonPropertyName("last_edited_time")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public DateTime LastEditedTime { get; set; } //The setter is used when deserializing.
    
    [JsonPropertyName("parent")]
    public NotionParentObject ParentObject { get; set; } = new();
    
    [JsonPropertyName("properties")]
    public Properties Properties { get; set; } = new();
}

public class NotionParentObject
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("database_id")]
    public string DatabaseId { get; set; } = string.Empty;
}

public class Properties
{
    public Product Product { get; set; } = new();
    public Status Status { get; set; } = new();
    public Author Author { get; set; } = new();
    public LastUpdated LastUpdated { get; set; } = new();
    public Number Number { get; set; } = new();
    public Created Created { get; set; } = new();
    public DueDate DueDate { get; set; } = new();
    public Link Link { get; set; } = new();
    public AssignedTo AssignedTo { get; set; } = new();
    public Modified Modified { get; set; } = new();
    public Version Version { get; set; } = new();
    public Schedule Schedule { get; set; } = new();
    public Type Type { get; set; } = new();
    public Title Title { get; set; } = new();
}

//--------------------------------------------------------------------------------

public class Annotations
{
    [JsonPropertyName("bold")]
    public bool Bold { get; set; }
    
    [JsonPropertyName("italic")]
    public bool Italic { get; set; }
    
    [JsonPropertyName("strikethrough")]
    public bool Strikethrough { get; set; }
    
    [JsonPropertyName("underline")]
    public bool Underline { get; set; }
    
    [JsonPropertyName("code")]
    public bool Code { get; set; }
    
    [JsonPropertyName("color")]
    public string Color { get; set; } = string.Empty;
}

public class AssignedTo
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("rich_text")]
    public List<RichText> RichText { get; set; } = new ();
}

public class Author
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("rich_text")]
    public List<RichText> RichText { get; set; } = new ();
}

public class Created
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("created_time")]
    public DateTime CreatedTime { get; set; }
}

public class Date
{
    [JsonPropertyName("start")]
    public DateTime Start { get; set; }
    
    [JsonPropertyName("end")]
    public object? End { get; set; }
    
    [JsonPropertyName("time_zone")]
    public object? TimeZone { get; set; }
}

public class DueDate
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("date")]
    public object? DueDateValue { get; set; }
}

public class LastUpdated
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("date")]
    public Date LastUpdatedDate { get; set; } = new();
}

public class Link
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

public class Modified
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("last_edited_time")]
    public DateTime LastEditedTime { get; set; }
}

public class Number
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("number")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int IssueNumber { get; set; } //The setter is used when deserializing.
}

public class Product
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("select")]
    public Select Select { get; set; } = new();
}

public class RichText
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("text")]
    public Text Text { get; set; } = new();
    
    [JsonPropertyName("annotations")]
    public Annotations Annotations { get; set; } = new();
    
    [JsonPropertyName("plain_text")]
    public string PlainText { get; set; } = string.Empty;
    
    [JsonPropertyName("href")]
    public object? Href { get; set; }
}

public class Schedule
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("date")]
    public object? ScheduleDate { get; set; }
}

public class Select
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("color")]
    public string Color { get; set; } = string.Empty;
}

public class Status
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("select")]
    public Select Select { get; set; } = new();
}

public class Text
{
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
    
    [JsonPropertyName("link")]
    public object? RedmineLink { get; set; }
}

public class Title
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<RichText> PageTitle { get; set; } = []; // The setter is used when deserializing.
}


public class Type
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string IssueType { get; set; } = string.Empty;
    
    [JsonPropertyName("select")]
    public Select Select { get; set; } = new();
}

public class Version
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("select")]
    public Select Select { get; set; } = new();
}