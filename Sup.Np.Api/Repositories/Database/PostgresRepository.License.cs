using Dapper;
using Sup.Common.Entities.Redmine;

namespace Sup.Np.Api.Repositories.Database;

public partial class PostgresRepository
{
    public async Task<int> InsertLicenseAsync(License license)
    {
        const string query = """
                             INSERT INTO license(key, product)
                             VALUES (@Key, @Product);
                             """;
        try
        {
            await _conn.OpenAsync();
            var result = await _conn.ExecuteAsync(query, license);
            return result;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<List<T>> GetLicensesAsync<T>(string productCode)
    {
        const string query = """
                             SELECT key AS Key, product as Product, auth_audience AS AuthAudience, signing_key as AuthSigningKey 
                             FROM license 
                             WHERE product = @Product;
                             """;
        try
        {
            await _conn.OpenAsync();
            var result = await _conn.QueryAsync<T>(query, new { Product = productCode});
            return result.ToList();
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }
}