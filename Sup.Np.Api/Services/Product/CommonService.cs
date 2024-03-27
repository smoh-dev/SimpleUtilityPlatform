using Sup.Common;
using Sup.Common.Configs;
using Sup.Common.Entities.Redmine;
using Sup.Common.Logger;
using Sup.Np.Api.Repositories.Database;

namespace Sup.Np.Api.Services.Product;

public class CommonService(SupLog log, IDbRepository db)
{
    private readonly SupLog _log = log.ForContext<CommonService>();
    private readonly IDbRepository _db = db;
    
    public async Task<EsConfigs?> GetEsConfigsAsync(string productCode)
    {
        try
        {
            var profiles = await _db.GetProfilesAsync<Profile>();
            var esUrl = profiles.FirstOrDefault(p => p.Entry == Consts.ProfileEntries.EsUrl)?.Value;
            var esUser = profiles.FirstOrDefault(p => p.Entry == Consts.ProfileEntries.EsUser)?.Value;
            var esPass = profiles.FirstOrDefault(p => p.Entry == Consts.ProfileEntries.EsPassword)?.Value;
            var esIndex = productCode switch
            {
                Consts.ProductCode.NpIssueLoader => Consts.EsIndexNames.IssueLoaderIndex,
                Consts.ProductCode.NpPageFixer => Consts.EsIndexNames.PageFixerLogIndex,
                Consts.ProductCode.NpPagePublisher => Consts.EsIndexNames.PagePublisherIndex,
                _ => ""
            };
            if (string.IsNullOrEmpty(esUrl) || string.IsNullOrEmpty(esUser) || string.IsNullOrEmpty(esPass))
                return null;
            return new EsConfigs(esUrl, esIndex, esUser, esPass);
        }
        catch (Exception ex)
        {
            _log.Fatal(ex, "{method_name} failed. {error_message}",
                nameof(GetEsConfigsAsync), ex.Message);
            return null;
        }
    }
}