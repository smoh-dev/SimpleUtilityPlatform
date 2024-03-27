namespace Sup.Np.PageFixer;

/// <summary>
/// Class for user info and  user profile
/// </summary>
public class PageFixerProfiles
{
    public string NotionApiUrl { get; set; } = string.Empty;
    public string NotionApiVersion { get; set; } = string.Empty;
    public string NotionDbId { get; set; } = string.Empty;
    public string NotionApiKey { get; set; } = string.Empty;
    public long MaxIssueLimit { get; set; }
    public long MinIssueLimit { get; set; }
}