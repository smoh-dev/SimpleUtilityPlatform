using Npgsql;

namespace Sup.Np.Api.Repositories.Database;

public partial class PostgresRepository : IDbRepository
{
    private readonly NpgsqlConnection _conn;

    public PostgresRepository(IConfiguration configs)
    {
        var dbSection = configs.GetSection("Database");
        var dbConnectionString = dbSection.GetValue<string>("ConnectionString");
        var dbHost = dbSection.GetValue<string>("Host");
        var dbPort = dbSection.GetValue<int>("Port");
        var dbUser = dbSection.GetValue<string>("User");
        var dbPassword = dbSection.GetValue<string>("Password");
        var dbName = dbSection.GetValue<string>("Database");

        if (!string.IsNullOrEmpty(dbConnectionString))
            _conn = new NpgsqlConnection(dbConnectionString);
        else
            _conn = new NpgsqlConnection(
                $"Host={dbHost};Port={dbPort};Username={dbUser};Password={dbPassword};Database={dbName};");
    }

}