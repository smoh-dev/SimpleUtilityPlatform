namespace Sup.Common.Entities.Redmine;

public class IssueWithPageId : Issue
{
    public string PageId { get; set; } = string.Empty;

    public IssueWithPageId()
    {
        
    }

    public IssueWithPageId(string pageId, Issue issue) : base(issue)
    {
        PageId = pageId;
    }
}