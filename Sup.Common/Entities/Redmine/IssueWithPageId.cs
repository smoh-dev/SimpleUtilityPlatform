namespace Sup.Common.Entities.Redmine;

public class IssueWithPageId : Issue
{
    public string PageId { get; set; } = string.Empty;
}