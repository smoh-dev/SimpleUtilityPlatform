using Dapper;
using Sup.Common.Entities.Redmine;

namespace Sup.Np.Api.Repositories.Database;

public partial class PostgresRepository
{
    public async Task<T?> GetPageAsync<T>(long issueNumber)
    {
        const string query = """
                             SELECT id        AS Id,
                                    issue_id  AS IssueId,
                                    posted_at AS PostedAt
                             FROM page
                             WHERE issue_id = @IssueNumber;
                             """;

        try
        {
            await _conn.OpenAsync();
            var result = await _conn.QueryFirstOrDefaultAsync<T>(query, new { issueNumber });
            return result;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<List<T>> GetPagesAsync<T>(List<long> issueIds)
    {
        const string query = """
                             SELECT id        AS Id,
                                    issue_id  AS IssueId,
                                    posted_at AS PostedAt
                             FROM page
                             WHERE issue_id = ANY(@IssueIds)
                             """;

        try
        {
            await _conn.OpenAsync();
            var result = await _conn.QueryAsync<T>(query, new {IssueIds = issueIds });
            return result.ToList();
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<List<T>> GetPagesAsync<T>(List<string> pageIds)
    {
        const string query = """
                             SELECT id        AS Id,
                                    issue_id  AS IssueId,
                                    posted_at AS PostedAt
                             FROM page
                             WHERE id = ANY(@PageIds)
                             """;

        try
        {
            await _conn.OpenAsync();
            var result = await _conn.QueryAsync<T>(query, new {PageIds = pageIds });
            return result.ToList();
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<int> InsertPagesAsync<T>(List<Page> pages)
    {
        const string query = """
                             INSERT INTO PAGE (id, issue_id, posted_at, account_id)
                             VALUES (@Id, @IssueId, @PostedAt, @AccountId);
                             """;

        try
        {
            await _conn.OpenAsync();
            var result = await _conn.ExecuteAsync(query, pages);
            return result;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<int> UpdatePagesAsync<T>(List<Page> pages)
    {
        const string query = """
                             UPDATE page
                             SET id        = @Id,
                                 issue_id  = @IssueId,
                                 posted_at = @PostedAt
                             WHERE issue_id = @IssueId;
                             """;
        
        try
        {
            await _conn.OpenAsync();
            var result = await _conn.ExecuteAsync(query, pages);
            return result;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }
}