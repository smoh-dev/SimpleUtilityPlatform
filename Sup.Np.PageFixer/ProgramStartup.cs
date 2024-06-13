using System.Text;
using System.Text.Json;
using Sup.Common;
using Sup.Common.Models.RequestParams;
using Sup.Common.Models.Responses;
using Sup.Common.Utils;

namespace Sup.Np.PageFixer;

public static class ProgramStartup
{
    
    /// <summary>
    /// Check license and auth info before start the IssueLoader.
    /// </summary>
    /// <param name="apiUrl"></param>
    /// <param name="licenseKey"></param>
    /// <exception cref="HttpRequestException">Failed to check license</exception>
    /// <returns></returns>
    public static async Task<CheckLicenseResponse?> CheckLicenseAsync(string apiUrl, string licenseKey)
    {
        try
        {
            using var client = new HttpClient();
            var requestUrl = $"{apiUrl}/Common/license";
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            var param = new CheckLicenseParam
            {
                ProductCode = Consts.ProductCode.NpPageFixer,
                HashedLicenseKey = new SupHash().Hash512(licenseKey)
            };
            var json = JsonSerializer.Serialize(param);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CheckLicenseResponse>(responseJson);

            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}