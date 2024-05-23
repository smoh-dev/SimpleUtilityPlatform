using System.Xml.XPath;
using Dapper;
using Npgsql;

namespace Sup.Mm.Api.Repositories;

public class PostgresRepository : IDbRepository
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
    
    public async Task<List<T>> GetNoteTagAsync<T>()
    {
        const string query = """
                             SELECT n.id AS Id, n.value AS Value, n.created_at AS CreatedAt, STRING_AGG(t.name, ',') AS Tags
                               FROM note n, note_tag nt, tag t
                              WHERE n.id = nt.note_id
                                AND nt.tag_id = t.id
                              GROUP BY n.id;
                             """;
        try
        {
            await _conn.OpenAsync();
            var result = await _conn.QueryAsync<T>(query);
            return result.ToList();
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }
}