using Sup.Common;
using Sup.Common.Kms;
using Sup.Common.TokenManager;
using Sup.Np.PageFixer;

#if DEBUG
const string configFileName = "appsettings.Development.json";
#else
const string configFileName = "appsettings.json";
#endif

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile(configFileName, optional: true, reloadOnChange: true);


// Check license
var apiUrl = builder.Configuration["ApiUrl"];
var licenseKey = builder.Configuration["LicenseKey"];
if (string.IsNullOrEmpty(apiUrl) || string.IsNullOrEmpty(licenseKey)) 
    throw new Exception("Information required for licence validation is missing.");
var license = await ProgramStartup.CheckLicenseAsync(apiUrl, licenseKey);
if (license is not { Success: true })
    throw new Exception("License is invalid.");


// Add AWS KMS client
var accessKey = Environment.GetEnvironmentVariable("ACCESS_KEY") ?? builder.Configuration.GetValue("AccessKey", "");
var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY") ?? builder.Configuration.GetValue("SecretKey", "");
if(string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
    throw new Exception("AWS configs are missing.");
var awsOptions = new AwsKmsOptions
{
    AccessKey = accessKey,
    SecretKey = secretKey,
    Region = Consts.Aws.Region,
    KeyId = license.KeyId
};
if(!awsOptions.IsValid()) throw new Exception("AWS configs are invalid.");
builder.Services.AddSingleton<AwsKmsEncryptor>(_=> new AwsKmsEncryptor(awsOptions));


// Register TokenManager.
var decryptedSigningKey = "";
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var kmsEncryptor = scope.ServiceProvider.GetRequiredService<AwsKmsEncryptor>();
    decryptedSigningKey = kmsEncryptor.DecryptStringAsync(license.SigningKey).Result;
}
builder.Services.AddSingleton<TokenManager>(_ =>
{
    var configs = new AuthConfigs
    {
        TokenUrl = license.TokenUrl,
        Audience = license.Audience,
        SigningKey = decryptedSigningKey
    };
    builder.Configuration.GetSection("OAuth").Bind(configs);
    return new TokenManager(configs);
});


// The BackgroundWorker works as a singleton, so it instantiates and
// uses the classes it needs internally without the need to DI.
builder.Services.AddSingleton<TokenManager>(_ =>
{
    var configs = new AuthConfigs();
    builder.Configuration.GetSection("OAuth").Bind(configs);
    return new TokenManager(configs);
});
builder.Services.AddHostedService<PageFixerWorker>();

var host = builder.Build();
host.Run();