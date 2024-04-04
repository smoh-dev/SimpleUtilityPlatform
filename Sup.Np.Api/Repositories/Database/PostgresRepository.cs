using Npgsql;

namespace Sup.Np.Api.Repositories.Database;

public partial class PostgresRepository : IDbRepository
{
    private readonly NpgsqlConnection _conn;

    public PostgresRepository(IConfiguration configs)
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
        
        _conn = new NpgsqlConnection(connString);
    }

}