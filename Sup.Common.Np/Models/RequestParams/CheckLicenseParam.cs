using System.Text.Json.Serialization;

namespace Sup.Common.Models.RequestParams;

public class CheckLicenseParam
{
    [JsonPropertyName("product_code")]
    public string ProductCode { get; set; } = string.Empty;
    [JsonPropertyName("hashed_license_key")]
    public string HashedLicenseKey { get; set; } = string.Empty;
}