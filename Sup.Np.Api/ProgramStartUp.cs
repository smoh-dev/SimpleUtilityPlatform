using Sup.Common;
using Sup.Common.Configs;
using Sup.Common.Entities.Redmine;
using Sup.Common.Kms;
using Sup.Np.Api.Repositories.Database;

namespace Sup.Np.Api;

public static class ProgramStartUp
{
    /// <summary>
    /// Load ES configs from the database.
    /// </summary>
    /// <param name="dbRepo"></param>
    /// <param name="kmsEnc"></param>
    /// <returns></returns>
    /// <exception cref="Exception">Failed connection test.</exception>
    public static EsConfigs? GetEsConfigs(in IDbRepository dbRepo, in AwsKmsEncryptor kmsEnc)
    {
        EsConfigs? esConfigs = null;
        try
        {
            var esUrl = dbRepo.GetProfileAsync<Profile>(Consts.ProfileEntries.EsUrl).Result?.Value;
            var esUser = dbRepo.GetProfileAsync<Profile>(Consts.ProfileEntries.EsUser).Result?.Value;
            var esPassword = dbRepo.GetProfileAsync<Profile>(Consts.ProfileEntries.EsPassword).Result?.Value;

            if (esUrl == null || esUser == null || esPassword == null)
            {
                Console.WriteLine("ES configs are missing.");
            }
            else
            {
                var decryptedPassword = kmsEnc.DecryptStringAsync(esPassword).Result;
                esConfigs = new EsConfigs(esUrl, Consts.EsIndexNames.NpApiLogIndex, esUser, decryptedPassword);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to load ES configs. {0}", ex);
        }

        return esConfigs;
    }

    /// <summary>
    /// Load OAuth configs from the database.
    /// </summary>
    /// <param name="dbRepo"></param>
    /// <param name="kmsEnc"></param>
    /// <param name="license"></param>
    /// <returns></returns>
    public static OAuthConfigs? GetOAuthConfigs(in IDbRepository dbRepo, in AwsKmsEncryptor kmsEnc, in License license)
    {
        OAuthConfigs oAuthConfigs = null;
        try
        {
            var oAuthAuthority = dbRepo.GetProfileAsync<Profile>(Consts.ProfileEntries.OAuthAuthority).Result?.Value;
            var oAuthAuthorizationUrl = dbRepo.GetProfileAsync<Profile>(Consts.ProfileEntries.OAuthAuthorizationUrl)
                .Result?.Value;
            var oAuthMetadataUrl =
                dbRepo.GetProfileAsync<Profile>(Consts.ProfileEntries.OAuthMetadataUrl).Result?.Value;
            var oAuthTokenUrl = dbRepo.GetProfileAsync<Profile>(Consts.ProfileEntries.OAuthTokenUrl).Result?.Value;
            var oAuthAudience = license.AuthAudience;
            var oAuthSigningKey = license.AuthSigningKey;
            var decryptedSigningKey = kmsEnc.DecryptStringAsync(oAuthSigningKey).Result;

            if (string.IsNullOrEmpty(oAuthAuthority) || string.IsNullOrEmpty(oAuthAuthorizationUrl) ||
                string.IsNullOrEmpty(oAuthMetadataUrl) || string.IsNullOrEmpty(oAuthTokenUrl) ||
                string.IsNullOrEmpty(oAuthAudience) || string.IsNullOrEmpty(decryptedSigningKey))
            {
                Console.WriteLine("OAuth configs are missing.");
            }
            else
            {
                oAuthConfigs = new OAuthConfigs
                {
                    Authority = oAuthAuthority,
                    AuthorizationUrl = oAuthAuthorizationUrl,
                    MetadataAddress = oAuthMetadataUrl,
                    TokenUrl = oAuthTokenUrl,
                    Audience = oAuthAudience,
                    SigningKey = decryptedSigningKey
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to load OAuth configs. {0}", ex);
        }

        return oAuthConfigs;
    }
}