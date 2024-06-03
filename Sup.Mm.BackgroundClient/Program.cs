
using Sup.Mm.BackgroundClient;
using Sup.Mm.BackgroundClient.Services;

var builder = Host.CreateApplicationBuilder(args);

#if DEBUG
const string configFileName = "appsettings.Development.json";
#else
const string configFileName = "appsettings.json";
#endif

builder.Configuration.AddJsonFile(configFileName, optional: true, reloadOnChange: true);
builder.Services.AddSingleton<AuthService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();