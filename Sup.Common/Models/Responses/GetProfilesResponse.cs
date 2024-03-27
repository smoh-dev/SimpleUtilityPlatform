using System.Text.Json.Serialization;
using Sup.Common.Entities.Redmine;

namespace Sup.Common.Models.Responses;

public class GetProfilesResponse : ApiResponse
{
    [JsonPropertyName("profiles")]
    public List<Profile> Profiles { get; set; } = [];

    public GetProfilesResponse()
    {
        
    }
    
    public GetProfilesResponse(List<Profile> profiles)
    {
        this.Profiles = profiles;
    }
    
    public GetProfilesResponse(bool result, int errorCode, string errorMessage) 
        : base(result, errorCode, errorMessage)
    {
        
    }
}