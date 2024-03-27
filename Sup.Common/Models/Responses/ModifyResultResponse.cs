using System.Text.Json.Serialization;

namespace Sup.Common.Models.Responses;

public class ModifyResultResponse : ApiResponse
{
    [JsonPropertyName("affected_row_count")]
    public int AffectedRowCount { get; set; }
    
    public ModifyResultResponse()
    {
        
    }
    
    public ModifyResultResponse(bool isSuccess, int errorCode, string errorMessage) : base(isSuccess, errorCode, errorMessage)
    {
        
    }
}