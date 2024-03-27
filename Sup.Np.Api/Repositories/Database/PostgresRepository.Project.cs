using Dapper;
using Sup.Common.Entities.Redmine;

namespace Sup.Np.Api.Repositories.Database;

public partial class PostgresRepository
{
    public async Task<List<T>> GetProjectsAsync<T>()
    {
        const string query = """
                             SELECT id   AS Id,
                                    name as Name
                             FROM project;
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

    public async Task<List<T>> GetProjectsAsync<T>(List<long> projectIds)
    {
        const string query = """
                             SELECT id   AS Id,
                                    name as Name
                             FROM project
                             WHERE id = ANY(@ProjectIds);
                             """;

        await _conn.OpenAsync();
        
        await _conn.CloseAsync();

        try
        {
            await _conn.OpenAsync();
            var result = await _conn.QueryAsync<T>(query, new { ProjectIds = projectIds });
            return result.ToList();

        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<T?> GetProjectAsync<T>(string projectName)
    {
        const string query = """
                             SELECT id   AS Id,
                                    name as Name
                             FROM project
                             WHERE name = @ProjectName;
                             """;

        try
        {
            await _conn.OpenAsync();
            var result = await _conn.QueryFirstOrDefaultAsync<T>(query, new { ProjectName = projectName });
            return result;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<int> InsertProjectAsync<T>(Project project)
    {
        const string query = """
                             INSERT INTO project(id, name)
                             VALUES (@Id, @Name);
                             """;

        try
        {
            await _conn.OpenAsync();
            var result = await _conn.ExecuteAsync(query, project);
            return result;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }
    
    public async Task<int> InsertProjectsAsync<T>(List<Project> projects)
    {
        const string query = """
                             INSERT INTO project(id, name)
                             VALUES (@Id, @Name);
                             """;

        try
        {
            await _conn.OpenAsync();
            var result = await _conn.ExecuteAsync(query, projects);
            return result;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }
    
    public async Task<int> UpdateProjectsAsync<T>(List<Project> projects)
    {
        const string query = """
                             UPDATE project
                             SET name = @Name
                             WHERE id = @Id;
                             """;

        try
        {
            await _conn.OpenAsync();
            var result = await _conn.ExecuteAsync(query, projects);
            return result;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }
}