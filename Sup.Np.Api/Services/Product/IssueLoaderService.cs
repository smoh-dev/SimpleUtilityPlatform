using Sup.Common;
using Sup.Common.Entities.Redmine;
using Sup.Common.Logger;
using Sup.Common.Models.RequestParams;
using Sup.Common.Models.Responses;
using Sup.Np.Api.Repositories.Database;

namespace Sup.Np.Api.Services.Product;

public class IssueLoaderService(SupLog log, IDbRepository db)
{
    private readonly SupLog _log = log.ForContext<IssueLoaderService>();
    private readonly IDbRepository _db = db;

    public async Task<ModifyResultResponse> PutProjectsAsync(PutProjectsParam param)
    {
        var result = 0;
        try
        {
            var projects = param.Projects
                .Select(project => new Project { Id = project.Id, Name = project.Name })
                .ToList();
            
            var projectIds = projects.Select(project => project.Id).ToList();
            var existingProjects = await _db.GetProjectsAsync<Project>(projectIds);
            
            var newProjects = projects;
            if (existingProjects.Count > 0)
            {
                var existingProjectIds = existingProjects.Select(project => project.Id).ToList();
                existingProjects = projects.Where(p => existingProjectIds.Contains(p.Id)).ToList();
                var updatedRowCount = await _db.UpdateProjectsAsync<Project>(existingProjects);
                result += updatedRowCount;
                _log.Debug("{method_name} is working. Updated {affected_row_count} projects.",
                    nameof(PutProjectsAsync), updatedRowCount);
                newProjects = projects.Where(project => !existingProjectIds.Contains(project.Id)).ToList();
            }
            
            var insertedRowCount = await _db.InsertProjectsAsync<int>(newProjects);
            result += insertedRowCount;
            _log.Debug("{method_name} is working. Inserted {affected_row_count} projects.",
                nameof(PutProjectsAsync), insertedRowCount);
            _log.Debug("{method_name} success. {affected_row_count} rows affected.",
                nameof(PutProjectsAsync), result);
            return new ModifyResultResponse { AffectedRowCount = result };
        }
        catch (Exception ex)
        {
            _log.Fatal(ex, "{method_name} failed. {error_message}",
                nameof(PutProjectsAsync), ex.Message);
            return new ModifyResultResponse(false, Consts.ErrorCode.DatabaseError, ex.Message);
        }
    }

    /// <summary>
    /// Update issues that already exist in the DB and inserted new issues that do not exist.
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task<ModifyResultResponse> UpsertIssuesAsync(PutIssuesParam param)
    {
        int result = 0;
        try
        {
            var issues = param.Issues
                .Select(issue => new Issue
                {
                    Id = issue.Id,
                    ProjectId = issue.Project.Id,
                    Type = issue.Type.Name,
                    Status = issue.Status.Name,
                    Priority = issue.Priority.Name,
                    AssignedTo = issue.AssignedTo.Name,
                    TargetVersion = issue.TargetVersion.Name,
                    CategoryName = issue.Category.Name,
                    ParentIssueId = issue.ParentIssue.Id,
                    Title = issue.Title,
                    CreatedOn = issue.CreatedOn,
                    UpdatedOn = issue.UpdatedOn,
                    Author = issue.Author.Name
                }).ToList();
            
            var issueIds = issues.Select(issue => issue.Id).ToList();
            var existingIssues = await _db.GetIssuesAsync<Issue>(issueIds);
            if (existingIssues is { Count: > 0 })
            {
                foreach (var newIssue in issues)
                {
                    var existingIssue = existingIssues.FirstOrDefault(issue => issue.Id == newIssue.Id);
                    if (existingIssue != null)
                    {
                        // Update all values except last_posted_on.
                        existingIssue.ProjectId = newIssue.ProjectId;
                        existingIssue.Type = newIssue.Type;
                        existingIssue.Status = newIssue.Status;
                        existingIssue.Priority = newIssue.Priority;
                        existingIssue.AssignedTo = newIssue.AssignedTo;
                        existingIssue.TargetVersion = newIssue.TargetVersion;
                        existingIssue.CategoryName = newIssue.CategoryName;
                        existingIssue.ParentIssueId = newIssue.ParentIssueId;
                        existingIssue.Title = newIssue.Title;
                        existingIssue.CreatedOn = newIssue.CreatedOn;
                        existingIssue.UpdatedOn = newIssue.UpdatedOn;
                        existingIssue.Author = newIssue.Author;
                    }
                }
            }
            
            var newIssues = issues;
            if (existingIssues != null && existingIssues.Count > 0)
            {
                var updatedRowCount= await _db.UpdateIssuesAsync<Issue>(existingIssues);
                result += updatedRowCount;
                _log.Debug("{method_name} is working. Updated {affected_row_count} issues.",
                    nameof(UpsertIssuesAsync), updatedRowCount);
                var existingIssueIds = existingIssues.Select(issue => issue.Id).ToList();
                newIssues = issues.Where(issue => !existingIssueIds.Contains(issue.Id)).ToList();
            }
            
            var insertedRowCount = await _db.InsertIssuesAsync<int>(newIssues);
            result += insertedRowCount;
            _log.Debug("{method_name} is working. Updated {affected_row_count} issues.",
                nameof(UpsertIssuesAsync), insertedRowCount);
            _log.Debug("{method_name} success. {affected_row_count} rows affected.",
                nameof(UpsertIssuesAsync), result);
            return new ModifyResultResponse { AffectedRowCount = result };
        }
        catch (Exception ex)
        {
            _log.Fatal(ex, "{method_name} failed. {error_message}",
                nameof(UpsertIssuesAsync), ex.Message);
            return new ModifyResultResponse(false, Consts.ErrorCode.DatabaseError, ex.Message);
        }
    }
}