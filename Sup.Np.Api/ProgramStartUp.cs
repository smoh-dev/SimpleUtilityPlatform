using System.Data;
using Npgsql;
using Sup.Common;
using Sup.Common.Configs;
using Sup.Common.Entities.Redmine;
using Sup.Common.Kms;
using Sup.Common.Utils;
using Sup.Np.Api.Repositories.Database;

namespace Sup.Np.Api;

public static class ProgramStartUp
{
    /// <summary>
    /// Load ES configs from the database.
    /// </summary>
    /// <param name="dbRepo"></param>
    /// <param name="kmsEnc"></param>
    /// <param name="configs"></param>
    /// <returns></returns>
    /// <exception cref="Exception">Failed connection test.</exception>
    public static EsConfigs? GetEsConfigs(in IDbRepository dbRepo, in AwsKmsEncryptor kmsEnc, IConfiguration configs)
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
}