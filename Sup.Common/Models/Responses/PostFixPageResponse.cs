namespace Sup.Common.Models.Responses;

public class PostFixPageResponse : ApiResponse
{
    public PostFixPageResponse()
    {
        
    }
    public PostFixPageResponse(bool isSuccess, int errorCode, string errorMessage) : base(isSuccess, errorCode, errorMessage)
    {
        
    }
    public PostFixPageResponse(ApiResponse apiResponse) : base(apiResponse)
    {
        
    }
}
