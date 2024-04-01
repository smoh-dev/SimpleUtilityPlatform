using Dapper;
using Sup.Common.Entities.Redmine;

namespace Sup.Np.Api.Repositories.Database;

public partial class PostgresRepository
{
    public async Task<T?> GetIssueAsync<T>(long issueNumber)
    {
        const string query = """
                             SELECT id              AS Id,
                                    project_id      AS ProjectId,
                                    type            AS Type,
                                    status          AS Status,
                                    priority        AS Priority,
                                    assigned_to     AS AssignedTo,
                                    target_version  AS TargetVersion,
                                    category_name   AS CategoryName,
                                    parent_issue_id AS ParentIssueId,
                                    title           AS Title,
                                    created_on      AS CreatedOn,
                                    updated_on      AS UpdatedOn,
                                    last_posted_on  AS LastPostedOn,
                                    author          AS Author
                               FROM issue
                              WHERE id = @IssueNumber
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

    public async Task<List<T>?> GetIssuesAsync<T>(List<long> issueNumbers)
    {
        const string query = """
                             SELECT id              AS Id,
                                    project_id      AS ProjectId,
                                    type            AS Type,
                                    status          AS Status,
                                    priority        AS Priority,
                                    assigned_to     AS AssignedTo,
                                    target_version  AS TargetVersion,
                                    category_name   AS CategoryName,
                                    parent_issue_id AS ParentIssueId,
                                    title           AS Title,
                                    created_on      AS CreatedOn,
                                    updated_on      AS UpdatedOn,
                                    last_posted_on  AS LastPostedOn,
                                    author          AS Author
                               FROM issue
                              WHERE id = ANY(@IssueIds)
                             """;

        try
        {
            await _conn.OpenAsync();
            var result = await _conn.QueryAsync<T>(query, new { IssueIds = issueNumbers });
            return result.ToList();
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<int> InsertIssueAsync<T>(Issue issue)
    {
        const string query = """
                             INSERT INTO issue(id, project_id, type, status, priority, assigned_to, target_version, category_name,
                                               parent_issue_id, title, created_on, author)
                             VALUES (@Id, @ProjectId, @Type, @Status, @Priority, @AssignedTo, @TargetVersion, @CategoryName,
                                     @ParentIssueId,@Title, @CreatedOn, @Author);
                             """;

        try
        {
            await _conn.OpenAsync();
            var result = await _conn.ExecuteAsync(query, issue);
            return result;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<int> InsertIssuesAsync<T>(List<Issue> issues)
    {
        const string query = """
                             INSERT INTO issue (id, project_id, type, status, priority, assigned_to, target_version, 
                                                category_name, parent_issue_id,title, created_on,
                                                updated_on, last_posted_on, author)
                             VALUES (@Id, @ProjectId, @Type, @Status, @Priority, @AssignedTo, @TargetVersion,
                                     @CategoryName, @ParentIssueId,@Title, @CreatedOn,
                                     @UpdatedOn, @LastPostedOn, @Author);
                             """;

        try
        {
            await _conn.OpenAsync();
            var result = await _conn.ExecuteAsync(query, issues);
            return result;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<int> UpdateIssueAsync<T>(Issue issue)
    {
        const string query = """
                             UPDATE public.issue
                             SET project_id      = @ProjectId,
                                 type            = @Type,
                                 status          = @Status,
                                 priority        = @Priority,
                                 assigned_to     = @AssignedTo,
                                 target_version  = @TargetVersion,
                                 category_name   = @CategoryName,
                                 parent_issue_id = @ParentIssueId,
                                 title           = @Title,
                                 created_on      = @CreatedOn,
                                 updated_on      = @UpdatedOn,
                                 author          = @Author
                             WHERE issue.id = @Id;
                             """;

        try
        {
            await _conn.OpenAsync();
            var result = await _conn.ExecuteAsync(query, issue);
            return result;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<int> UpdateIssuesAsync<T>(List<Issue> issues)
    {
        const string query = """
                             UPDATE public.issue
                             SET project_id      = @ProjectId,
                                 type            = @Type,
                                 status          = @Status,
                                 priority        = @Priority,
                                 assigned_to     = @AssignedTo,
                                 target_version  = @TargetVersion,
                                 category_name   = @CategoryName,
                                 parent_issue_id = @ParentIssueId,
                                 title           = @Title,
                                 created_on      = @CreatedOn,
                                 updated_on      = @UpdatedOn,
                                 author          = @Author
                             WHERE issue.id = @Id;
                             """;

        try
        {
            await _conn.OpenAsync();
            var result = await _conn.ExecuteAsync(query, issues);
            return result;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<List<T>> GetIssuesToPostPageAsync<T>()
    {
        const string query = """
                             SELECT p.id              AS PageId,
                                    i.id              AS Id,
                                    i.project_id      AS ProjectId,
                                    i.type            AS Type,
                                    i.status          AS Status,
                                    i.priority        AS Priority,
                                    i.assigned_to     AS AssignedTo,
                                    i.target_version  AS TargetVersion,
                                    i.category_name   AS CategoryName,
                                    i.parent_issue_id AS ParentIssueId,
                                    i.title           AS Title,
                                    i.created_on      AS CreatedOn,
                                    i.updated_on      AS UpdatedOn,
                                    i.last_posted_on  AS LastPostedOn,
                                    i.author          AS Author
                             FROM issue i
                                      LEFT OUTER JOIN page p ON i.id = p.issue_id
                             WHERE i.last_posted_on IS NULL;
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

    public async Task<List<T>> GetIssuesToPatchPageAsync<T>()
    {
        const string query = """
                             SELECT p.id              AS PageId,
                                    i.id              AS Id,
                                    i.project_id      AS ProjectId,
                                    i.type            AS Type,
                                    i.status          AS Status,
                                    i.priority        AS Priority,
                                    i.assigned_to     AS AssignedTo,
                                    i.target_version  AS TargetVersion,
                                    i.category_name   AS CategoryName,
                                    i.parent_issue_id AS ParentIssueId,
                                    i.title           AS Title,
                                    i.created_on      AS CreatedOn,
                                    i.updated_on      AS UpdatedOn,
                                    i.last_posted_on  AS LastPostedOn,
                                    i.author          AS Author
                             FROM issue AS i
                                      JOIN page AS p ON i.id = p.issue_id
                             WHERE (last_posted_on < updated_on
                                 OR last_posted_on > (SELECT posted_at
                                                      FROM page
                                                      WHERE issue_id = i.id));
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

    public async Task<List<T>> GetUnpublishedIssuesAsync<T>()
    {
        const string query = """
                             select page.issue_id
                             from issue
                                      right join page on issue.id = page.issue_id
                             where issue.id is null;
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