using Serilog;
using Serilog.Sinks.Elasticsearch;
using Sup.Common.Configs;

namespace Sup.Common.Logger;

public class SupLog
{
    private readonly bool _isEnableFileLog = false;
    private readonly bool _isEnableEsLog = false;
    private readonly ILogger _log;

    public SupLog(bool isEnableFileLog, EsConfigs? esConfigs)
    {
        var logConfig = new LoggerConfiguration();

        if (isEnableFileLog)
        {
            _isEnableFileLog = true;
            const string logPath = $"./logs/{Consts.EsIndexNames.NpApiLogIndex}.txt";
            logConfig.WriteTo.File(path: logPath,rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, flushToDiskInterval: TimeSpan.FromSeconds(1));
        }
        if (esConfigs != null
            && !string.IsNullOrEmpty(esConfigs.EsUrl)
            && !string.IsNullOrEmpty(esConfigs.EsIndex)
            && !string.IsNullOrEmpty(esConfigs.EsUser)
            && !string.IsNullOrEmpty(esConfigs.EsPassword))
        {
            _isEnableEsLog = true;
            var indexFormat = esConfigs.EsIndex + "{0:yyyy.MM.dd}";
            logConfig.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(esConfigs.EsUrl))
            {
                MinimumLogEventLevel = Serilog.Events.LogEventLevel.Verbose,
                ModifyConnectionSettings = conn =>
                    conn.BasicAuthentication(esConfigs.EsUser, esConfigs.EsPassword),
                IndexFormat = indexFormat
            }); 
        }
        logConfig.WriteTo.Console().MinimumLevel.Verbose();
        
        _log = logConfig.CreateLogger();
    }

    public SupLog(ILogger log)
    {
        _log = log;
    }
    
    ~SupLog()
    {
        Log.CloseAndFlush();
    }
    
    public SupLog ForContext<T>()
    {
        return new SupLog(_log.ForContext<T>()); 
    }
    
    public bool IsEnabledFileLog() => _isEnableFileLog;
    public bool IsEnabledEsLog() => _isEnableEsLog;
    
    public void Verbose(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        _log.Verbose(exception, messageTemplate, propertyValues);
    }
    public void Verbose(string messageTemplate, params object[] propertyValues)
    {
        _log.Verbose(messageTemplate, propertyValues);
    }

    public void Debug(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        _log.Debug(exception, messageTemplate, propertyValues);
    }
    public void Debug(string messageTemplate, params object[] propertyValues)
    {
        _log.Debug(messageTemplate, propertyValues);
    }

    public void Info(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        _log.Information(exception, messageTemplate, propertyValues);
    }
    public void Info(string messageTemplate, params object[] propertyValues)
    {
        _log.Information(messageTemplate, propertyValues);
    }

    public void Warn(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        _log.Warning(exception, messageTemplate, propertyValues);
    }
    public void Warn(string messageTemplate, params object[] propertyValues)
    {
        _log.Warning(messageTemplate, propertyValues);
    }

    public void Error(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        _log.Error(exception, messageTemplate, propertyValues);
    }
    public void Error(string messageTemplate, params object[] propertyValues)
    {
        _log.Error(messageTemplate, propertyValues);
    }

    public void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        _log.Fatal(exception, messageTemplate, propertyValues);
    }
    public void Fatal(string messageTemplate, params object[] propertyValues)
    {
        _log.Fatal(messageTemplate, propertyValues);
    }
}
