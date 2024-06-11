using System.Text.Json.Serialization;

namespace Sup.Common.Models.RequestParams;

public class CheckLicenseParam
{
    [JsonPropertyName("product_code")]
    public string ProductCode { get; set; } = string.Empty;
    [JsonPropertyName("license_key")]
    public string LicenseKey { get; set; } = string.Empty;
}