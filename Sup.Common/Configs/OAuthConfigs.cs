namespace Sup.Common.Configs;

public class OAuthConfigs
{
    public string MetadataAddress { get; set; } = string.Empty;
    public string Authority { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SigningKey { get; set; } = string.Empty;
    public string AuthorizationUrl { get; set; } = string.Empty;
    public string TokenUrl  { get; set; } = string.Empty;
}