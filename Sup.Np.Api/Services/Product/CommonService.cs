using System.Security.Cryptography;
using System.Text;
using Sup.Common;
using Sup.Common.Configs;
using Sup.Common.Entities.Redmine;
using Sup.Common.Logger;
using Sup.Np.Api.Repositories.Database;

namespace Sup.Np.Api.Services.Product;

public class CommonService(SupLog log, IDbRepository db)
{
    private readonly SupLog _log = log.ForContext<CommonService>();
    private readonly IDbRepository _db = db;
    
    public async Task<EsConfigs?> GetEsConfigsAsync(string productCode)
    {
        try
        {
            var profiles = await _db.GetProfilesAsync<Profile>();
            var esUrl = profiles.FirstOrDefault(p => p.Entry == Consts.ProfileEntries.EsUrl)?.Value;
            var esUser = profiles.FirstOrDefault(p => p.Entry == Consts.ProfileEntries.EsUser)?.Value;
            var esPass = profiles.FirstOrDefault(p => p.Entry == Consts.ProfileEntries.EsPassword)?.Value;
            var esIndex = productCode switch
            {
                Consts.ProductCode.NpIssueLoader => Consts.EsIndexNames.IssueLoaderIndex,
                Consts.ProductCode.NpPageFixer => Consts.EsIndexNames.PageFixerLogIndex,
                Consts.ProductCode.NpPagePublisher => Consts.EsIndexNames.PagePublisherIndex,
                _ => ""
            };
            if (string.IsNullOrEmpty(esUrl) || string.IsNullOrEmpty(esUser) || string.IsNullOrEmpty(esPass))
                return null;
            return new EsConfigs(esUrl, esIndex, esUser, esPass);
        }
        catch (Exception ex)
        {
            _log.Fatal(ex, "{method_name} failed. {error_message}",
                nameof(GetEsConfigsAsync), ex.Message);
            return null;
        }
    }
    
    public async Task<string> GenerateLicenseAsync(string productCode)
    {
        var licenseKey = "";
        try
        {
            var randomBytes = new byte[20];
            RandomNumberGenerator.Fill(randomBytes);
            const string base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            StringBuilder result = new StringBuilder((randomBytes.Length + 4) / 5 * 8);
            int byteIndex = 0, bitIndex = 0;
            while (byteIndex < randomBytes.Length)
            {
                int currentByte = randomBytes[byteIndex];
                int digitIndex = bitIndex + 5 > 8 ? 8 - bitIndex : 5;
                var digit = currentByte >> (8 - (bitIndex + digitIndex)) & 0x1F;
                result.Append(base32Chars[digit]);
                bitIndex += 5;
                if (bitIndex < 8) continue;
                bitIndex -= 8;
                byteIndex++;
            }
            licenseKey = result.ToString();

            var license = new License
            {
                Key = licenseKey,
                Product = productCode,
            };
            var insertedRowCount = await _db.InsertLicenseAsync(license);
            if (insertedRowCount == 0)
            {
                _log.Error("{method_name} failed. InsertedRowCount is 0.", nameof(GenerateLicenseAsync));
                return "";
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return licenseKey;
    }
    
    public async Task<int> CheckLicenseAsync(string productCode, string licenseKey)
    {
        try
        {
            var license = await _db.GetLicenseAsync<License>(productCode, licenseKey);
            return license == null ? 0 : 1;
        }
        catch (Exception ex)
        {
            _log.Fatal(ex, "{method_name} failed. {error_message}",
                nameof(CheckLicenseAsync), ex.Message);
            return 0;
        }
    }
}