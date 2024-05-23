namespace Sup.Common.Models.Responses;

public class GetRedmineInfoResponse : ApiResponse
{
    public string RedmineUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public List<long> TargetProjectIds { get; set; } = [];
    public int LoadIssueLimit { get; set; } = 0;
    
    public GetRedmineInfoResponse()
    {
        
    }

    public GetRedmineInfoResponse(bool success, int errorCode, string message) : base(success, errorCode, message)
    {
        
    }
}