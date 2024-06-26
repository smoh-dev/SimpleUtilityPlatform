using System.Collections;
using Sup.Common;
using Sup.Common.Entities.QueryParams;
using Sup.Common.Entities.Redmine;
using Sup.Common.Logger;
using Sup.Common.Models.RequestParams;
using Sup.Common.Models.Responses;
using Sup.Np.Api.Repositories.Database;

namespace Sup.Np.Api.Services.Product;

public class PagePublisherService(SupLog log, IDbRepository db)
{
    private readonly SupLog _log = log.ForContext<PagePublisherService>();
    private readonly IDbRepository _db = db;

    /// <summary>
    /// Insert new pages. If it already exists, update it.
    /// TODO: Transactions?
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task<ModifyResultResponse> UpsertPagesAsync(PutPagesParam param)
    {
        var result = 0;
        try
        {
            var pages = param.Pages
                .Select(p => new Page
                {
                    Id = p.PageId,
                    IssueId = p.IssueNumber,
                    PostedAt = p.PostedAt,
                }).ToList();

            var issueIds = pages.Select(p => p.IssueId).ToList();
            var existingPages = await _db.GetPagesAsync<Page>(issueIds);

            var newPages = pages;
            if (existingPages.Count > 0)
            {
                var existingIssueIds = existingPages.Select(p => p.IssueId).ToList();
                existingPages = pages.Where(p => existingIssueIds.Contains(p.IssueId)).ToList();
                var updatedRowCount = await _db.UpdatePagesAsync<Page>(existingPages);
                result += updatedRowCount;
                _log.Debug("{method_name} is working. Updated {affected_row_count} pages.",
                    nameof(UpsertPagesAsync), updatedRowCount);
                newPages = pages.Where(p => !existingIssueIds.Contains(p.IssueId)).ToList();
            }

            if (newPages.Count > 0)
            {
                var insertedRowCount = await _db.InsertPagesAsync<int>(newPages);
                result += insertedRowCount;
                _log.Debug("{method_name} is working. Insert {affected_row_count} pages.",
                    nameof(UpsertPagesAsync), insertedRowCount);
            }

            var updatedIssues = pages.Select(p => new UpdateIssueLastPostedOnParam(p.IssueId, p.PostedAt)).ToList();
            var updatedIssueCount = await _db.UpdateIssueLastPostedOnAsync<int>(updatedIssues);

            _log.Debug("{method_name} success. {affected_row_count} rows affected.",
                nameof(UpsertPagesAsync), result);
            return new ModifyResultResponse { AffectedRowCount = result };
        }
        catch (Exception ex)
        {
            _log.Fatal(ex, "{method_name} failed. {error_message}",
                nameof(UpsertPagesAsync), ex.Message);
            return new ModifyResultResponse(false, Consts.ErrorCode.DatabaseError, ex.Message);
        }
    }

    public async Task<GetIssuesToPublishResponse> GetIssuesToPublishAsync()
    {
        var result = new GetIssuesToPublishResponse();
        try
        {
            var issuesToPost = await _db.GetIssuesToPostPageAsync<IssueWithPageId>();
            if (issuesToPost.Count > 0)
            {
                _log.Debug("{method_name} is working. Found {issue_count} issues that need to be published.",
                    nameof(GetIssuesToPublishAsync), issuesToPost.Count);
                result.IssuesToUpdate.AddRange(issuesToPost.Select(i => new IssueToPublish(i)).ToList());
            }

            var issuesToPatch = await _db.GetIssuesToPatchPageAsync<IssueWithPageId>();
            if (issuesToPatch.Count > 0)
            {
                _log.Debug("{method_name} is working. Found {issue_count} issues that need to be updated.",
                    nameof(GetIssuesToPublishAsync), issuesToPost.Count);
                result.IssuesToUpdate.AddRange(issuesToPatch.Select(i => new IssueToPublish(i)).ToList());
            }

            _log.Debug("{method_name} success. {issue_count} issues found.",
                nameof(GetIssuesToPublishAsync), result.IssuesToUpdate.Count);
            return result;
        }
        catch (Exception ex)
        {
            _log.Fatal(ex, "{method_name} failed. {error_message}",
                nameof(GetIssuesToPublishAsync), ex.Message);
            return new GetIssuesToPublishResponse(false, Consts.ErrorCode.DatabaseError, ex.Message);
        }
    }
}