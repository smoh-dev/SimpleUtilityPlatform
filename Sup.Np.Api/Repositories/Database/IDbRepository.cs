using System.Collections;
using Sup.Common.Entities.QueryParams;
using Sup.Common.Entities.Redmine;

namespace Sup.Np.Api.Repositories.Database;

public interface IDbRepository
{
    #region Issue
    /// <summary>
    /// Get issue from DB.
    /// </summary>
    /// <param name="issueNumber"></param>
    /// <returns></returns>
    public Task<T?> GetIssueAsync<T>(long issueNumber);

    /// <summary>
    /// Get issues from DB.
    /// </summary>
    /// <param name="issueNumbers"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Task<List<T>?> GetIssuesAsync<T>(List<long> issueNumbers);
    
    /// <summary>
    /// Insert a new row into the issue table.
    /// </summary>
    /// <param name="issue"></param>
    /// <returns>Inserted row count.</returns>
    public Task<int> InsertIssueAsync<T>(Issue issue);
    
    /// <summary>
    /// Insert multiple issues into the issue table.
    /// </summary>
    /// <param name="issues"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Task<int> InsertIssuesAsync<T>(List<Issue> issues);
    
    /// <summary>
    /// Update a row in the issue table.
    /// </summary>
    /// <param name="issue"></param>
    /// <returns>Updated row count.</returns>
    public Task<int> UpdateIssueAsync<T>(Issue issue);

    /// <summary>
    /// Update multiple rows in the issue table.
    /// </summary>
    /// <param name="issues"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Task<int> UpdateIssuesAsync<T>(List<Issue> issues);
    
    /// <summary>
    /// Get issues that require new notion pages to be created.
    /// </summary>
    /// <returns>Retrieved list of issues. If failed, return null.</returns>
    public Task<List<T>> GetIssuesToPostPageAsync<T>();
    
    /// <summary>
    /// Get issues that require the notion page to be updated.
    /// </summary>
    /// <returns>Retrieved list of issues. If failed, return null.</returns>
    public Task<List<T>> GetIssuesToPatchPageAsync<T>();

    /// <summary>
    /// Gets issue numbers with page information but no issue information.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Task<List<T>> GetUnpublishedIssuesAsync<T>();

    /// <summary>
    /// Update the published time since the page corresponding to the issue was published.
    /// </summary>
    /// <param name="param"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Task<int> UpdateIssueLastPostedOnAsync<T>(List<UpdateIssueLastPostedOnParam> param);
    #endregion Issue
    
    #region Page
    /// <summary>
    /// Get page from DB.
    /// </summary>
    /// <param name="issueNumber"></param>
    /// <returns></returns>
    public Task<T?> GetPageAsync<T>(long issueNumber);
    
    /// <summary>
    /// Get the pages by issue IDs.
    /// </summary>
    /// <param name="issueIds"></param>
    /// <returns></returns>
    public Task<List<T>> GetPagesAsync<T>(List<long> issueIds);
    
    /// <summary>
    /// Get the pages by page IDs.
    /// </summary>
    /// <param name="pageIds"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Task<List<T>> GetPagesAsync<T>(List<string> pageIds);
    
    /// <summary>
    /// Insert new pages.
    /// </summary>
    /// <param name="pages"></param>
    /// <returns>Inserted row count.</returns>
    public Task<int> InsertPagesAsync<T>(List<Page> pages);
    
    /// <summary>
    /// Update rows in the page table.
    /// </summary>
    /// <param name="pages"></param>
    /// <returns>Updated row count.</returns>
    public Task<int> UpdatePagesAsync<T>(List<Page> pages);
    #endregion Page
    
    #region Profile
    public Task<List<T>> GetProfilesAsync<T>();
    public Task<T?> GetProfileAsync<T>(string entry);
    #endregion Profile
    
    #region Project
    /// <summary>
    /// Get all projects from DB and save them into private list.
    /// This method called when the service starts.
    /// </summary>
    /// <returns></returns>
    public Task<List<T>> GetProjectsAsync<T>();
    
    /// <summary>
    /// Get the projects by project IDs.
    /// </summary>
    /// <param name="projectIds"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Task<List<T>> GetProjectsAsync<T>(List<long> projectIds);
    
    /// <summary>
    /// Get project by project name.
    /// </summary>
    /// <param name="projectName"></param>
    /// <returns>If noe exists, return 0.</returns>
    public Task<T?> GetProjectAsync<T>(string projectName);
    
    /// <summary>
    /// Insert a new project into DB.
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    public Task<int> InsertProjectAsync<T>(Project project); 
    
    /// <summary>
    /// Insert multiple projects into DB.
    /// </summary>
    /// <param name="projects"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Task<int> InsertProjectsAsync<T>(List<Project> projects);

    /// <summary>
    /// Update multiple projects in the project table.
    /// </summary>
    /// <param name="projects"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Task<int> UpdateProjectsAsync<T>(List<Project> projects);

    #endregion Project
    
    #region License
    /// <summary>
    /// Insert new license to db.
    /// </summary>
    /// <param name="license"></param>
    /// <returns></returns>
    public Task<int> InsertLicenseAsync(License license);
    /// <summary>
    /// Get License from db. 
    /// </summary>
    /// <param name="productCode"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Task<T?> GetLicenseAsync<T>(string productCode, string licenseKey);
    #endregion License
}