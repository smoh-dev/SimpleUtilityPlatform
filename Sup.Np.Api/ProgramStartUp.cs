using Npgsql;
using Sup.Common;
using Sup.Common.Configs;
using Sup.Common.Utils;

namespace Sup.Np.Api;

public static class ProgramStartUp
{
    /// <summary>
    /// Load ES configs from the database.
    /// </summary>
    /// <param name="configs"></param>
    /// <returns></returns>
    /// <exception cref="Exception">Failed connection test.</exception>
    public static EsConfigs? GetEsConfigs(IConfiguration configs)
    {
        var dbSection = configs.GetSection("Database");
        var dbConnectionString = dbSection.GetValue<string>("ConnectionString"); // Not used yet.
        var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? dbSection.GetValue<string>("Host");
        var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? dbSection.GetValue<string>("Port");
        var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? dbSection.GetValue<string>("User");
        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? dbSection.GetValue<string>("Password");
        var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? dbSection.GetValue<string>("Database");

        var connString = !string.IsNullOrEmpty(dbConnectionString)
            ? dbConnectionString
            : $"Host={dbHost};Port={dbPort};Username={dbUser};Password={dbPassword};Database={dbName};";

        using var conn = new NpgsqlConnection(connString);
        try
        {
            conn.Open();

            const string query = "SELECT * FROM profile";
            string esUrl = string.Empty, esUser = string.Empty, esPassword = string.Empty;

            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (reader.GetString(0) == Consts.ProfileEntries.EsUrl)
                    esUrl = reader.GetString(1);
                else if (reader.GetString(0) == Consts.ProfileEntries.EsUser)
                    esUser = reader.GetString(1);
                else if (reader.GetString(0) == Consts.ProfileEntries.EsPassword)
                    esPassword = reader.GetString(1);
            }

            var encryptKey = Environment.GetEnvironmentVariable("ENCRYPT_KEY") 
                             ?? configs.GetValue<string>("EncryptKey");
            if (string.IsNullOrEmpty(encryptKey))
                throw new InvalidOperationException("Encrypt key is not set.");

            var enc = new Encrypter(encryptKey);
            if (!string.IsNullOrEmpty(esUrl) && !string.IsNullOrEmpty(esUser) && !string.IsNullOrEmpty(esPassword))
                return new EsConfigs(esUrl, Consts.EsIndexNames.NpApiLogIndex, esUser, enc.Decrypt(esPassword));
            return null;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to connect to database. {ex.Message}", ex);
        }
    }
}