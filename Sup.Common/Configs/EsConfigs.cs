using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sup.Common.Configs;

public class EsConfigs
{
    [JsonPropertyName("es_url")]
    public string EsUrl { get; set; }
    
    [JsonPropertyName("es_index")]
    public string EsIndex { get; set; }
    
    [JsonPropertyName("es_user")]
    public string EsUser { get; set; }
    
    [JsonPropertyName("es_password")]
    public string EsPassword { get; set; }

    public EsConfigs()
    {
        EsUrl = EsIndex = EsPassword = EsUser = string.Empty;
    }
    public EsConfigs(string productCode, string apiBaseUrl)
    {
        try
        {
            using var client = new HttpClient();
            var requestUrl = $"{apiBaseUrl}/common/profiles/es/{productCode}";
            var response = client.GetAsync(requestUrl).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            var configs = JsonSerializer.Deserialize<EsConfigs>(json);
            EsIndex = configs == null ? string.Empty : configs.EsIndex;
            EsUrl = configs == null ? string.Empty : configs.EsUrl;
            EsUser = configs == null ? string.Empty : configs.EsUser;
            EsPassword = configs == null ? string.Empty : configs.EsPassword;
        }
        catch (Exception)
        {
            EsUrl = EsIndex = EsPassword = EsUser = string.Empty;
        }
    }
    
    public EsConfigs(string esUrl, string esIndex, string esUser, string esPassword)
    {
        EsUrl = esUrl;
        EsIndex = esIndex;
        EsUser = esUser;
        EsPassword = esPassword;
    }
}    
