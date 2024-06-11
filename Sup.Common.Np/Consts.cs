namespace Sup.Common;

/// <summary>
/// A constant class commonly used.
/// </summary>
public static class Consts
{
    /// <summary>
    /// A constant class for error codes. 
    /// </summary>
    public static class ErrorCode
    {
        public const int OperationFailed = 100000;
        public const int DatabaseError = 800000;
        public const int Unknown = 999999;
    }

    /// <summary>
    /// Profile entries.
    /// </summary>
    public static class ProfileEntries
    {

        public const string EsUrl = "ES_URL";
        public const string EsUser = "ES_USER";
        public const string EsPassword = "ES_PASSWORD";

        public const string RedmineUrl = "REDMINE_URL";
        public const string RedmineApiKey = "REDMINE_API_KEY";
        
        public const string NotionDbId = "NOTION_DB_ID";
        public const string NotionApiKey = "NOTION_API_KEY";
        public const string NotionApiUrl = "NOTION_API_URL";
        public const string NotionApiVersion = "NOTION_API_VERSION";
        
        public const string LoaderRecoverDuration = "LOADER_RECOVER_DURATION";
        /// <summary>
        /// Comma separated project ids.
        /// </summary>
        public const string LoaderTargetProjectIds = "LOADER_TARGET_PROJECT_IDS";
        public const string LoaderSchedule = "LOADER_SCHEDULE"; // json format

        public const string PublisherSchedule = "PUBLISHER_SCEDULE"; // json format
        
        public const string FixerMaxIssueLimit = "FIXER_MAX_ISSUE_LIMIT";
        public const string FixerMinIssueLimit = "FIXER_MIN_ISSUE_LIMIT";
    }

    /// <summary>
    /// Index names for Elasticsearch.
    /// </summary>
    public static class EsIndexNames
    {
        public const string NpApiLogIndex = "log-np-api-";
        public const string PageFixerLogIndex = "log-np-page-fixer-";
        public const string IssueLoaderIndex = "log-np-issue-loader-";
        public const string PagePublisherIndex = "log-np-page-publisher-";
    }

    /// <summary>
    /// Product codes.
    /// </summary>
    public static class ProductCode
    {
        public const string NpApi = "NPAPI";
        public const string NpPageFixer = "NPPXR";
        public const string NpIssueLoader = "NPILR";
        public const string NpPagePublisher = "NPPPB";
    }

    public static class Auth
    {
        public const string ScopeLicense = "license";
        public const string PolicyLicense = "PolocyLicense"; // "license
    }

}