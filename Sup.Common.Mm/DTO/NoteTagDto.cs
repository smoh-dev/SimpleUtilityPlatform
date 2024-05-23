using System.Text.Json.Serialization;

namespace Sup.Mm.Common.DTO;

public class NoteTagDto : IResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    
    [JsonPropertyName("error_code")]
    public int ErrorCode { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("note_id")]
    public long Id { get; set; }
    
    [JsonPropertyName("note_value")]
    public string Value { get; set; } = string.Empty;
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("tags")]
    public string Tags { get; set; } = string.Empty;
}