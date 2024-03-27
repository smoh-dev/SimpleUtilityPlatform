using System.Data;
using Npgsql;
using Sup.Common;
using Sup.Common.Configs;
using Sup.Common.Utils;

namespace Sup.Np.Api;

public static class ProgramStartUp
{
    /// <summary>
    /// Load configs from environment variables and appsettings.json.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NoNullAllowedException">Missing require field.</exception>
    /// <exception cref="InvalidCastException">Failed parse config value.</exception>
    public static ApiConfigs LoadConfigs()
    {
        var config = new ApiConfigs();

        // Load configs from environment variables
        var env = Environment.GetEnvironmentVariables();
        config.DbConnectionString = env["DB_CONNECTION_STRING"]?.ToString() ?? string.Empty;
        config.DbHost = env["DB_HOST"]?.ToString() ?? string.Empty;
        config.DbPort = env["DB_PORT"]?.ToString() ?? string.Empty;
        config.DbUser = env["DB_USER"]?.ToString() ?? string.Empty;
        config.DbName = env["DB_NAME"]?.ToString() ?? string.Empty;
        config.DbPassword = env["DB_PASSWORD"]?.ToString() ?? string.Empty;
        config.UseSwagger = env["USE_SWAGGER"]?.ToString() ?? string.Empty;

        // Load configs from appsettings.json
        var appSettings = new ConfigurationBuilder()
#if _DEBUG
            .AddJsonFile("appsettings.Development.json", false, true)
#else
            .AddJsonFile("appsettings.json", false, true)
#endif
            .Build();
        if (config.DbConnectionString == string.Empty)
            config.DbConnectionString = appSettings["Database:ConnectionString"] ?? string.Empty;
        if (config.DbHost == string.Empty)
            config.DbHost = appSettings["Database:Host"] ?? string.Empty;
        if (config.DbPort == string.Empty)
            config.DbPort = appSettings["Database:Port"] ?? string.Empty;
        if (config.DbUser == string.Empty)
            config.DbUser = appSettings["Database:User"] ?? string.Empty;
        if (config.DbPassword == string.Empty)
            config.DbPassword = appSettings["Database:Password"] ?? string.Empty;
        if (config.DbName == string.Empty)
            config.DbName = appSettings["Database:Database"] ?? string.Empty;
        if (config.UseSwagger == string.Empty)
            config.UseSwagger = appSettings["UseSwagger"] ?? string.Empty;

        // Check if configs are valid
        if (config.DbConnectionString == string.Empty &&
            (config.DbHost == string.Empty || config.DbPort == string.Empty || config.DbUser == string.Empty ||
             config.DbPassword == string.Empty || config.DbName == string.Empty))
            throw new NoNullAllowedException("Database connection info is not set");

        return config;
    }

    /// <summary>
    /// Load ES configs from the database.
    /// </summary>
    /// <param name="configs"></param>
    /// <returns></returns>
    /// <exception cref="Exception">Failed connection test.</exception>
    public static EsConfigs? GetEsConfigs(IConfiguration configs)
    {
        var dbSection = configs.GetSection("Database");
        var dbConnectionString = dbSection.GetValue<string>("ConnectionString");
        var dbHost = dbSection.GetValue<string>("Host");
        var dbPort = dbSection.GetValue<int>("Port");
        var dbUser = dbSection.GetValue<string>("User");
        var dbPassword = dbSection.GetValue<string>("Password");
        var dbName = dbSection.GetValue<string>("Database");
        
        string connString;
        if (!string.IsNullOrEmpty(dbConnectionString))
            connString = dbConnectionString;
        else
            connString = $"Host={dbHost};Port={dbPort};Username={dbUser};Password={dbPassword};Database={dbName};";
        
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
            
            var encryptKey = configs.GetValue<string>("EncryptKey");
            if(string.IsNullOrEmpty(encryptKey))
                throw new InvalidOperationException("Encrypt key is not set.");
            
            var enc = new Encrypter(encryptKey);
            if(!string.IsNullOrEmpty(esUrl) && !string.IsNullOrEmpty(esUser) && !string.IsNullOrEmpty(esPassword))
                return new EsConfigs(esUrl, Consts.EsIndexNames.NpApiLogIndex, esUser, enc.Decrypt(esPassword));
            return  null;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to connect to database. {ex.Message}", ex);
        }
    }

}