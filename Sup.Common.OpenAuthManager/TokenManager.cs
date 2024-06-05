using System.Collections;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Sup.Common.TokenManager;

public class AuthConfigs
{
    public string TokenUrl { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SigningKey { get; set; } = string.Empty;
}

public class TokenManager
{
    private readonly string _tokenEndpoint;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private string? _accessToken = string.Empty;
    private DateTime _tokenExpiration = DateTime.UtcNow;

    public TokenManager(AuthConfigs configs)
    {
        _tokenEndpoint = configs.TokenUrl;
        _clientId = configs.Audience;
        _clientSecret = configs.SigningKey;
        if (string.IsNullOrEmpty(_tokenEndpoint) || string.IsNullOrEmpty(_clientId) ||
            string.IsNullOrEmpty(_clientSecret))
            throw new ArgumentException("OAuth configuration is not valid.");
    }

    /// <summary>
    /// Create a new HttpClient with Bearer token.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception">Failed to get access token.</exception>
    public async Task<HttpClient> GetHttpClientAsync()
    {
        var token = await GetAccessTokenAsync();
        if (string.IsNullOrEmpty(token))
            throw new Exception("Failed to get access token.");
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    /// <summary>
    /// Get new access token or refresh the token from OAuth server.
    /// </summary>
    /// <returns>null if error.</returns>
    private async Task<string?> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiration) 
            return _accessToken;
        
        try
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
        catch (Exception)
        {
            return null;
        }

        return _accessToken;
    }
}