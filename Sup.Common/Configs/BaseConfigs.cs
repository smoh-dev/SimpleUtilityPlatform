namespace Sup.Common.Configs;

public class BaseConfig
{
    public EsConfigs? EsConfigs { get; set; }
    public string LicenseKey { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
    public string RedmineUrl { get; set; } = string.Empty;
    public string RedmineApiKey { get; set; } = string.Empty;
}