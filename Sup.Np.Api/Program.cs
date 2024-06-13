using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sup.Common;
using Sup.Common.Configs;
using Sup.Common.Entities.Redmine;
using Sup.Common.Kms;
using Sup.Common.Logger;
using Sup.Np.Api;
using Sup.Np.Api.Repositories.Database;
using Sup.Np.Api.Services.Product;


// Create a builder for the web host
var builder = WebApplication.CreateBuilder(args);


// Bind configs
#if DEBUG
const string configFileName = "appsettings.Development.json";
#else
const string configFileName = "appsettings.json";
#endif
builder.Configuration.AddJsonFile(configFileName, optional: true, reloadOnChange: true);


// Add DB repository, services, and controllers
builder.Services.AddScoped<IDbRepository, PostgresRepository>();
builder.Services.AddScoped<CommonService>();
builder.Services.AddScoped<IssueLoaderService>();
builder.Services.AddScoped<PagePublisherService>();
builder.Services.AddControllers();


// Check license and get oAuth configs
License? license = null;
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    IConfiguration configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile(configFileName)
        .Build();
    var licenseKey = configuration["LicenseKey"];
    if(string.IsNullOrEmpty(licenseKey)) throw new Exception("License key is missing.");
    var dbRepo = scope.ServiceProvider.GetRequiredService<IDbRepository>();
    license = dbRepo.GetLicenseAsync<License>(Consts.ProductCode.NpApi, licenseKey).Result;
}
if(license == null) throw new Exception("License is invalid.");


// Add AWS KMS client
var awsOptions = builder.Configuration.GetSection("AWS").Get<AwsKmsOptions>();
if(awsOptions == null) throw new Exception("AWS configs are missing.");
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var dbRepo = scope.ServiceProvider.GetRequiredService<IDbRepository>();
    var profile = await dbRepo.GetProfileAsync<Profile>(Consts.ProfileEntries.AwsKmsKeyId);
    if(profile == null) throw new Exception("Profile(AWS key id) is missing.");
    awsOptions.KeyId = profile.Value;
}
if(!awsOptions.IsValid()) throw new Exception("AWS configs are invalid.");
builder.Services.AddSingleton<AwsKmsEncryptor>(_=> new AwsKmsEncryptor(awsOptions));


// Add logger
builder.Services.AddScoped<SupLog>(_ =>
{
    EsConfigs? esConfigs = null;
    using (var scope = builder.Services.BuildServiceProvider().CreateScope())
    {
        var dbRepo = scope.ServiceProvider.GetRequiredService<IDbRepository>();
        var kmsEnc = scope.ServiceProvider.GetRequiredService<AwsKmsEncryptor>();
        esConfigs = ProgramStartUp.GetEsConfigs(in dbRepo, in kmsEnc);
    }
    
    return new SupLog(true, esConfigs);
});


// Add oAuth
OAuthConfigs? oAuthConfigs = null;
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var dbRepo = scope.ServiceProvider.GetRequiredService<IDbRepository>();
    var kmsEnc = scope.ServiceProvider.GetRequiredService<AwsKmsEncryptor>();
    oAuthConfigs = ProgramStartUp.GetOAuthConfigs(in dbRepo, in kmsEnc, in license);
}
if(oAuthConfigs == null) throw new Exception("OAuth configs are missing.");
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.MetadataAddress = oAuthConfigs.MetadataAddress;
    opt.Authority = oAuthConfigs.Authority;
    opt.Audience = oAuthConfigs.Audience;
    opt.RequireHttpsMetadata = false;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = opt.Authority,
        ValidateAudience = true,
        ValidAudience = "account",
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(oAuthConfigs.SigningKey))
    };
    opt.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Authentication failed.", context.Exception);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated.");
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy(Consts.Auth.PolicyLicense, policy =>
        policy.RequireAssertion(context =>
        {
            var scopeClaim = context.User.FindFirst(claim => claim.Type == "scope");
            if (scopeClaim == null) return false;
            var scopes = scopeClaim.Value.Split(' ');
            return scopes.Any(s => s.Equals(Consts.Auth.ScopeLicense, StringComparison.OrdinalIgnoreCase));
        }));
    opt.AddPolicy(Consts.Auth.PolicyKms, policy =>
        policy.RequireAssertion(context =>
        {
            var scopeClaim = context.User.FindFirst(claim => claim.Type == "scope");
            if (scopeClaim == null) return false;
            var scopes = scopeClaim.Value.Split(' ');
            return scopes.Any(s => s.Equals(Consts.Auth.ScopeKms, StringComparison.OrdinalIgnoreCase));
        }));
});


// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    // Add bearer auth
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Name = "Authorization",
        In = ParameterLocation.Header,
        Flows = new OpenApiOAuthFlows
        {
            ClientCredentials = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(oAuthConfigs.AuthorizationUrl),
                TokenUrl = new Uri(oAuthConfigs.TokenUrl),
            },
        },
        OpenIdConnectUrl = new Uri(oAuthConfigs.MetadataAddress),
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


// Build the web host
var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


// Apply swagger
#if DEBUG
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.OAuthClientId(oAuthConfigs.Audience);
    c.OAuthClientSecret(oAuthConfigs.SigningKey);
});
#else
if(Convert.ToBoolean(Environment.GetEnvironmentVariable("ENABLE_SWAGGER")))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.OAuthClientId(oAuthConfigs.Audience);
        c.OAuthClientSecret(oAuthConfigs.SigningKey); 
    });
}
#endif


// Run app.
app.Run();