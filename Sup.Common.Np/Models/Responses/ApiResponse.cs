using System.Text.Json.Serialization;

namespace Sup.Common.Models.Responses;

/// <summary>
/// Default fields included in API responses.
/// </summary>
public class ApiResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; } = true;
    
    [JsonPropertyName("error_code")]
    public int ErrorCode { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    public ApiResponse() { }
    
    public ApiResponse(ApiResponse result)
    {
        Success = result.Success;
        ErrorCode = result.ErrorCode;
        Message = result.Message;
    }
    
    public ApiResponse(bool isSuccess, int errorCode, string message)
    {
        Success = isSuccess;
        ErrorCode = errorCode;
        this.Message = message;
    }

}