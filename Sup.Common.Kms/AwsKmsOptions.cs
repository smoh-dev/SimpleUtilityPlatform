namespace Sup.Common.Kms;

public class AwsKmsOptions
{
    public string KeyId { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(KeyId) 
               && !string.IsNullOrEmpty(AccessKey) 
               && !string.IsNullOrEmpty(SecretKey) 
               && !string.IsNullOrEmpty(Region);
    }
}