namespace Sup.Common.Entities.QueryParams;

public class UpdateIssueLastPostedOnParam
{
    public long IssueNumber { get; set; }
    public DateTime LastPostedOn { get; set; }

    public UpdateIssueLastPostedOnParam(long issueNumber, DateTime lastPostedOn)
    {
        IssueNumber = issueNumber;
        LastPostedOn = lastPostedOn;
    }
}