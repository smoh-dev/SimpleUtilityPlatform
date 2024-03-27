using Sup.Np.IssueLoader;

string configFileName = "";
#if DEBUG
configFileName = "appsettings.Development.json";
#else
configFileName = "appsettings.json";
#endif
var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile(configFileName, optional: true, reloadOnChange: true);

// The BackgroundWorker works as a singleton, so it instantiates and
// uses the classes it needs internally without the need to DI.
builder.Services.AddHostedService<IssueLoaderWorker>();

var host = builder.Build();
host.Run();