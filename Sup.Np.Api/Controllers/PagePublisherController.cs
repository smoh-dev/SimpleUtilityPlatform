using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Sup.Common.Logger;
using Sup.Common.Models.RequestParams;
using Sup.Np.Api.Services.Product;

namespace Sup.Np.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PagePublisherController(SupLog log, PagePublisherService svc) : ControllerBase
{
    private readonly SupLog _log = log.ForContext<PagePublisherController>();
    private readonly PagePublisherService _svc = svc;
    private readonly Stopwatch _sw = new();
    
    [HttpPut("pages")]
    public async Task<IActionResult> PutPagesAsync([FromBody] PutPagesParam param)
    {
        _sw.Restart();
        var result = await _svc.UpsertPagesAsync(param);
        _sw.Stop();
        
        _log.Verbose("{api_name} took {running_time}ms.", 
            nameof(PutPagesAsync), _sw.ElapsedMilliseconds);

        if(!result.Success)
            return BadRequest(result);
        if (result.AffectedRowCount == 0)
            return NoContent();
        return Ok(result);
    }

    [HttpGet("issues")]
    public async Task<IActionResult> GetPagesAsync()
    {
        _sw.Restart();
        var result = await _svc.GetIssuesToPublishAsync();
        _sw.Stop();
        
        _log.Verbose("{api_name} took {running_time}ms.", 
            nameof(GetPagesAsync), _sw.ElapsedMilliseconds);

        if(!result.Success)
            return BadRequest(result);
        if (result.IssuesToUpdate.Count == 0)
            return NoContent();
        return Ok(result);
    }
}