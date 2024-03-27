using Dapper;

namespace Sup.Np.Api.Repositories.Database;

public partial class PostgresRepository
{
    public async Task<List<T>> GetProfilesAsync<T>()
    {
        const string query = """
                             SELECT entry AS Entry,
                                    value AS Value
                             FROM profile;
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