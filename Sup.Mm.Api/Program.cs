using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sup.Mm.Api.Repositories;
using Sup.Mm.Api.Services;

var builder = WebApplication.CreateBuilder(args);

#if DEBUG
const string configFileName = "appsettings.Development.json";
#else
const string configFileName = "appsettings.json";
#endif

builder.Configuration.AddJsonFile(configFileName, optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddScoped<IDbRepository, PostgresRepository>();
builder.Services.AddScoped<NoteService>();
builder.Services.AddControllers();

// Add oAuth
var oAuthConfigs = builder.Configuration.GetSection("OAuth");
var metadataAddress = oAuthConfigs["MetadataAddress"];
var authority = oAuthConfigs["Authority"];
var audience = oAuthConfigs["Audience"];
var signingKey = oAuthConfigs["SigningKey"];
var authorizationUrl = oAuthConfigs["AuthorizationUrl"];
var tokenUrl = oAuthConfigs["TokenUrl"];
if (string.IsNullOrEmpty(metadataAddress) 
    || string.IsNullOrEmpty(authority) 
    || string.IsNullOrEmpty(audience)
    || string.IsNullOrEmpty(signingKey)
    || string.IsNullOrEmpty(authorizationUrl) 
    || string.IsNullOrEmpty(tokenUrl))
    throw new Exception("OAuth configs are missing.");
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.MetadataAddress = metadataAddress;
    opt.Authority = authority;
    opt.Audience = audience;
    opt.RequireHttpsMetadata = false;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = opt.Authority,
        ValidateAudience = true,
        ValidAudience = "account",
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
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
builder.Services.AddAuthorization();

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
                AuthorizationUrl = new Uri(authorizationUrl),
                TokenUrl = new Uri(tokenUrl),
            },
        },
        OpenIdConnectUrl = new Uri(metadataAddress),
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.OAuthClientId(audience);
        c.OAuthClientSecret(signingKey); 
    });
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();