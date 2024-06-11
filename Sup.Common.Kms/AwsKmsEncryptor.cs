using System.Text;
using Amazon;
using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;

namespace Sup.Common.Kms;

public class AwsKmsEncryptor(AwsKmsOptions awsOptions)
{
    private readonly string _keyId = awsOptions.KeyId;
    private readonly AmazonKeyManagementServiceClient _kmsClient = new (
        awsOptions.AccessKey, 
        awsOptions.SecretKey, 
        RegionEndpoint.GetBySystemName(awsOptions.Region));

    public async Task<string> EncryptStringAsync(string input)
    {
        var request = new EncryptRequest
        {
            KeyId = _keyId,
            Plaintext = new MemoryStream(Encoding.UTF8.GetBytes(input))
        };

        var response = await _kmsClient.EncryptAsync(request);
        var encryptedResult = response.CiphertextBlob.ToArray();
        var encryptedString = Convert.ToBase64String(encryptedResult);
        return encryptedString;
    }
    
    public async Task<string> DecryptStringAsync(string input)
    {
        var encryptedBytes = Convert.FromBase64String(input);
        var request = new DecryptRequest
        {
            CiphertextBlob = new MemoryStream(encryptedBytes)
        };
        var response = await _kmsClient.DecryptAsync(request);
        var decryptedResult = Encoding.UTF8.GetString(response.Plaintext.ToArray());
        return decryptedResult;
    }
}