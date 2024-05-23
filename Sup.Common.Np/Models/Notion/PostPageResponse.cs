using System.Text.Json.Serialization;

namespace Sup.Common.Models.Notion;

public class PostPageResponse : NotionPage
{
    [JsonPropertyName("id")]
    public string PostedPageId { get; set; } = string.Empty;
}