using System.Text.Json.Serialization;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Sup.Common.Models.Notion;

public class NotionPage
{
    [JsonPropertyName("parent")]
    public ParentDatabase DatabaseInfo { get; set; } = new();
    
    [JsonPropertyName("properties")]
    public PageProperties Properties { get; set; } = new();
    
    [JsonIgnore]
    public string ExistingPageId { get; set; } = string.Empty;

    public NotionPage()
    {
        
    }

    public NotionPage(string notionDbId, string pageId,
        long issueNumber, string title, string status, string type, string link, string author, string assignedTo)
    {
        DatabaseInfo = new ParentDatabase { DatabaseId = notionDbId };
        ExistingPageId = pageId; // Empty if new page.
        
        Properties.Number = new NumberProperty { Number = issueNumber };
        Properties.Title.RichTextValues[0].Value.Content = title;
        Properties.Status.Value.Name = status;
        Properties.Type.Value.Name = type;
        Properties.Link.UrlValue = link;
        Properties.Author.RichTextValues[0].Value.Content = author;
        Properties.AssignedTo.RichTextValues[0].Value.Content = assignedTo;
    }
}

public class ParentDatabase
{
    [JsonPropertyName("database_id")]
    public string DatabaseId { get; set; } = string.Empty;
}

public class PageProperties
{
    [JsonPropertyName("Title")]
    public TitleProperty Title { get; set; } = new();
    
    [JsonPropertyName("Number")]
    public NumberProperty Number { get; set; } = new();
    
    [JsonPropertyName("Status")]
    public StatusProperty Status { get; set; } = new();
    
    [JsonPropertyName("Type")]
    public TypeProperty Type { get; set; } = new();
    
    [JsonPropertyName("Link")]
    public LinkProperty Link { get; set; } = new();
    
    [JsonPropertyName("Author")]
    public AuthorProperty Author { get; set; } = new();
    
    [JsonPropertyName("Assigned to")]
    public AssignedToProperty AssignedTo { get; set; } = new();
}

public class TitleProperty
{
    [JsonPropertyName("title")]
    public RichTextType[] RichTextValues { get; set;}

    public TitleProperty()
    {
        RichTextValues = new RichTextType[1];
        RichTextValues[0] = new RichTextType();
    }
}

public class RichTextType
{
    [JsonPropertyName("type")]
    public string TypeName { get; set; } = "text";
    
    [JsonPropertyName("text")]
    public TextType Value { get; set; } = new();
}

public class TextType
{
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

public class NumberProperty
{
    [JsonPropertyName("number")]
    public long Number { get; set; }
}

public class StatusProperty
{
    [JsonPropertyName("select")]
    public SelectType Value { get; set; } = new();
}

public class SelectType
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = String.Empty;
}

public class TypeProperty
{
    [JsonPropertyName("select")]
    public SelectType Value { get; set; } = new();
}

public class LinkProperty
{
    [JsonPropertyName("url")]
    public string UrlValue { get; set; } = string.Empty;
}

public class AuthorProperty
{
    [JsonPropertyName("rich_text")]
    public RichTextType[] RichTextValues { get; set; }

    public AuthorProperty()
    {
        RichTextValues = new RichTextType[1];
        RichTextValues[0] = new RichTextType();
    
    }
}

public class AssignedToProperty
{
    [JsonPropertyName("rich_text")]
    public RichTextType[] RichTextValues { get; set; }

    public AssignedToProperty()
    {
        RichTextValues = new RichTextType[1];
        RichTextValues[0] = new RichTextType();
    }
}
