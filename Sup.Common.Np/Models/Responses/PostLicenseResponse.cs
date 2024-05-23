using System.Text.Json.Serialization;

namespace Sup.Common.Models.Responses;

public class PostLicenseResponse : ApiResponse
{
    [JsonPropertyName("license_key")]
    public string LicenseKey { get; set; } = string.Empty;
    
    [JsonPropertyName("user_id")]
    public long UserId { get; set; }
    
    [JsonPropertyName("product_code")]
    public string ProductCode { get; set; } = string.Empty;
    
    [JsonPropertyName("start_on")]
    public DateTime StartOn { get; set; }
    
    [JsonPropertyName("end_on")]
    public DateTime EndOn { get; set; }

    public PostLicenseResponse()
    {
        
    }

    public PostLicenseResponse(bool isSuccess, int errorCode, string message) 
        : base(isSuccess, errorCode, message)
    {
        
    }
}