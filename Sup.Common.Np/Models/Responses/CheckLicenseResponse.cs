namespace Sup.Common.Models.Responses;

public class CheckLicenseResponse : ApiResponse
{

    public string Audience { get; set; } = string.Empty;
    public string SigningKey { get; set; } = string.Empty;
    
    public CheckLicenseResponse()
    {
        
    }

    public CheckLicenseResponse(bool success, int errorCode, string message) : base(success, errorCode, message)
    {
        
    }
}