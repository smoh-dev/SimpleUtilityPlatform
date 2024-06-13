using System.Data;
using System.Text;
using System.Text.Json;
using Sup.Common;
using Sup.Common.Configs;
using Sup.Common.Kms;
using Sup.Common.Logger;
using Sup.Common.Models.RequestParams;
using Sup.Common.TokenManager;
using Sup.Np.PageFixer.Models;
using Sup.Np.PageFixer.Services;

namespace Sup.Np.PageFixer;

public class PageFixerWorker : BackgroundService
{
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly SupLog _log;
    private readonly PageFixerProfiles _profiles;
    private readonly ApiService _apiSvc;
    private readonly NotionService _notionSvc;
    private readonly List<NotionPage> _retryPages = [];
    
    public PageFixerWorker(
        IConfiguration configs,
        TokenManager tokenManager, 
        AwsKmsEncryptor kmsEncryptor,
        IHostApplicationLifetime appLifetime
        )
    {
        _appLifetime = appLifetime;
        
        // Create logger and api service.
        var apiUrl = configs["ApiUrl"];
        if (string.IsNullOrEmpty(apiUrl))
            throw new NoNullAllowedException("Api url is not set in appsettings.json");
        var esConfigs = new EsConfigs(Consts.ProductCode.NpIssueLoader, apiUrl);
        esConfigs.EsPassword = kmsEncryptor.DecryptStringAsync(esConfigs.EsPassword).Result;
        var log = new SupLog(true, esConfigs);
        if (log.IsEnabledEsLog())
            log.Info("Elasticsearch log enabled.");
        _apiSvc = new ApiService(log, tokenManager, apiUrl);
        
        // Load profiles.       
        _profiles = _apiSvc.GetProfilesAsync().Result;
        
        // Create notion service and test.
        _notionSvc = new NotionService(log, _profiles);
        if (!_notionSvc.ConnectionTestAsync().Result)
            throw new Exception("Failed to connect to Notion API.");
        
        _log = log.ForContext<PageFixerWorker>();
        _log.Info("{class_name} created.", nameof(PageFixerWorker));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var index = _profiles.MinIssueLimit;
        // Fix pages.
        while (!stoppingToken.IsCancellationRequested
               && index < _profiles.MaxIssueLimit)
        {
            var pages = await _notionSvc.GetIssuePagesAsync(index, index += 100);
            if (pages.Count > 0)
            {
                var putPageResult =  await _apiSvc.PutPagesAsync(pages);
                if (!putPageResult) _retryPages.AddRange(pages);
            }
            await Task.Delay(500, stoppingToken); // To avoid API rate limit exceeded.
        }
        
        // TODO: Retry failed pages.
        if(_retryPages.Count > 0)
            await _apiSvc.PutPagesAsync(_retryPages);

        _log.Debug("Verified all the ranges of NotionPages set in the profile.");
        _log.Debug("PageFixerWorker is stopping.");
        _appLifetime.StopApplication();
    }
}