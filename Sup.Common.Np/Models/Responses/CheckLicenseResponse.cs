using System.Text.Json.Serialization;

namespace Sup.Common.Models.Responses;

public class CheckLicenseResponse : ApiResponse
{

    [JsonPropertyName("audience")]
    public string Audience { get; set; } = string.Empty;
    [JsonPropertyName("signing_key")]
    public string SigningKey { get; set; } = string.Empty;
    [JsonPropertyName("token_url")]
    public string TokenUrl { get; set; } = string.Empty;
    [JsonPropertyName("key_id")]
    public string KeyId { get; set; } = string.Empty;

    public CheckLicenseResponse()
    {
        
    }

    public CheckLicenseResponse(bool success, int errorCode, string message) : base(success, errorCode, message)
    {
        
    }
}