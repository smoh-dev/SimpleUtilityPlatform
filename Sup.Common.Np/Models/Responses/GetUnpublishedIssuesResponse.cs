using System.Text.Json.Serialization;

namespace Sup.Common.Models.Responses;

public class GetUnpublishedIssuesResponse : ApiResponse
{
    [JsonPropertyName("issue_numbers")]
    public List<long> IssueNumbers { get; set; } = new();
    
    public GetUnpublishedIssuesResponse()
    {
        
    }

    public GetUnpublishedIssuesResponse(bool success, int errorCode, string message) : base(success, errorCode, message)
    {
        
    }
}