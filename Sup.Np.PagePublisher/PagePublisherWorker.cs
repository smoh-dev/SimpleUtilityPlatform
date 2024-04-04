using System.Data;
using Sup.Common;
using Sup.Common.Configs;
using Sup.Common.Logger;
using Sup.Common.Utils;
using Sup.Np.PagePublisher.Services;

namespace Sup.Np.PagePublisher;

public class PagePublisherWorker : BackgroundService
{
    private readonly int _interval;
    private readonly PagePublisherProfiles _profiles;
    private readonly ApiService _apiSvc;
    private readonly NotionService _notionSvc;

    public PagePublisherWorker(IConfiguration configs)
    {
#if DEBUG
        _interval = 1000 * 5;
#else
        _interval = 1000 * 60;
#endif

        // Validate.
        var apiHost = Environment.GetEnvironmentVariable("API_HOST");
        var apiPort = Environment.GetEnvironmentVariable("API_PORT");
        var apiUrl = !string.IsNullOrEmpty(apiHost) ? $"http://{apiHost}:{apiPort}" : configs["ApiUrl"];
        if (string.IsNullOrEmpty(apiUrl))
            throw new NoNullAllowedException("Api url is not set.");

        // Create loader and api service.
        var encKey = Environment.GetEnvironmentVariable("ENCRYPT_KEY") ?? configs["EncryptKey"];
        if (encKey == null)
            throw new NoNullAllowedException("EncryptKey is not set.");
        var enc = new Encrypter(encKey);
        var esConfigs = new EsConfigs(Consts.ProductCode.NpIssueLoader, apiUrl);
        esConfigs.EsPassword = enc.Decrypt(esConfigs.EsPassword);
        var log = new SupLog(true, esConfigs);
        if (log.IsEnabledEsLog())
        {
            log.Verbose("{es_url}({es_index})/{es_user}@{es_password}"
                , esConfigs.EsUrl, esConfigs.EsIndex, esConfigs.EsUser, esConfigs.EsPassword.Length);
            log.Info("Elasticsearch log enabled.");
        }
        _apiSvc = new ApiService(log, apiUrl);

        // Load profiles.       
        _profiles = _apiSvc.GetProfilesAsync().Result;

        // Create notion service and test.
        _notionSvc = new NotionService(log, _profiles);
        if (!_notionSvc.ConnectionTestAsync().Result)
            throw new Exception("Failed to connect to Notion API.");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var issuesToPublish = await _apiSvc.GetIssuesToPublishAsync();
            var publishResult = await _notionSvc.PublishPagesAsync(issuesToPublish);
            await _apiSvc.PutPagesAsync(publishResult);
            await Task.Delay(_interval, stoppingToken);
        }
    }
}