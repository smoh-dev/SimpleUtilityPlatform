using Sup.Common;
using Sup.Common.Logger;
using Sup.Common.Utils;
using Sup.Np.Api;
using Sup.Np.Api.Repositories.Database;
using Sup.Np.Api.Services.Product;

// Create a builder for the web host
var builder = WebApplication.CreateBuilder(args);

#if DEBUG
const string configFileName = "appsettings.Development.json";
#else
const string configFileName = "appsettings.json";
#endif

builder.Configuration.AddJsonFile(configFileName, optional: true, reloadOnChange: true);

builder.Services.AddScoped<IDbRepository, PostgresRepository>();

builder.Services.AddScoped<CommonService>();
builder.Services.AddScoped<IssueLoaderService>();
builder.Services.AddScoped<PagePublisherService>();

builder.Services.AddScoped<SupLog>(_ =>
{
    IConfiguration configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile(configFileName)
        .Build();
    var esConfigs = ProgramStartUp.GetEsConfigs(configuration);
    return new SupLog(true, esConfigs);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Build the web host
var app = builder.Build();

#if DEBUG
app.UseSwagger();
app.UseSwaggerUI();
#else
if(Convert.ToBoolean(Environment.GetEnvironmentVariable("ENABLE_SWAGGER")))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
#endif

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();