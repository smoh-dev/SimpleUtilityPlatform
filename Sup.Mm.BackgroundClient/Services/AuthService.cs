using System.Collections;
using System.Text.Json;


namespace Sup.Mm.BackgroundClient.Services;

public class AuthService
{
    private readonly string _tokenEndpoint;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private string? _accessToken = string.Empty;
    private DateTime _tokenExpiration = DateTime.UtcNow;
    
    public AuthService(IConfiguration configs)
    {
        var oAuthConfigs = configs.GetSection("OAuth");
        _tokenEndpoint = oAuthConfigs["TokenUrl"] ?? string.Empty;
        _clientId = oAuthConfigs["Audience"] ?? string.Empty;
        _clientSecret = oAuthConfigs["SigningKey"] ?? string.Empty;
        if (string.IsNullOrEmpty(_tokenEndpoint) || string.IsNullOrEmpty(_clientId) ||
            string.IsNullOrEmpty(_clientSecret))
            throw new ArgumentException("OAuth configuration is not valid.");
    }    
    
    public async Task<string?> GetAccessTokenAsync()
    {
        if (string.IsNullOrEmpty(_accessToken) || DateTime.UtcNow >= _tokenExpiration)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _tokenEndpoint);
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            using var client = new HttpClient(); 
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<Hashtable>(responseContent);
            if (tokenResponse == null) return null;
            _accessToken = tokenResponse["access_token"]?.ToString();
            var expiresIn = Convert.ToInt32(tokenResponse["expires_in"]?.ToString());
            expiresIn = expiresIn < 60 ? 300 : expiresIn;
            _tokenExpiration = DateTime.UtcNow.AddSeconds(expiresIn - 60); // 60초의 버퍼 추가
        }

        return _accessToken;
    }
}