using System.Text.Json.Serialization;

namespace Sup.Common.Models.RequestParams;

public class PostLicenseParam
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
}