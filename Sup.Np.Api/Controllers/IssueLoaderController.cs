
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sup.Common.Logger;
using Sup.Common.Models.RequestParams;
using Sup.Np.Api.Services.Product;

namespace Sup.Np.Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class IssueLoaderController(SupLog log, IssueLoaderService svc ) : ControllerBase
{
    private readonly Stopwatch _sw = new();

    [HttpPut("projects")]
    public async Task<IActionResult> PutProjectsAsync([FromBody] PutProjectsParam param)
    {
        _sw.Restart();
        var result = await svc.PutProjectsAsync(param);
        _sw.Stop();
        
        log.Verbose("{api_name} took {running_time}ms.", 
            nameof(PutProjectsAsync), _sw.ElapsedMilliseconds);

        if(!result.Success)
            return BadRequest(result);
        if (result.AffectedRowCount == 0)
            return NoContent();
        return Ok(result);
    }

    [HttpPut("issues")]
    public async Task<IActionResult> PutIssuesAsync([FromBody] PutIssuesParam param)
    {
        _sw.Restart();
        var result = await svc.UpsertIssuesAsync(param);
        _sw.Stop();
        
        log.Verbose("{api_name} took {running_time}ms.", 
            nameof(PutIssuesAsync), _sw.ElapsedMilliseconds);
        
        if(!result.Success)
            return BadRequest(result);
        if (result.AffectedRowCount == 0)
            return NoContent();
        return Ok(result);
    }
    
    [HttpGet("issues/unpublished")]
    public async Task<IActionResult> GetUnpublishedIssuesAsync()
    {
        _sw.Restart();
        var result = await svc.GetUnpublishedIssuesAsync();
        _sw.Stop();
        
        log.Verbose("{api_name} took {running_time}ms.", 
            nameof(GetUnpublishedIssuesAsync), _sw.ElapsedMilliseconds);
        
        if(!result.Success)
            return BadRequest(result);
        if (result.IssueNumbers.Count == 0)
            return NoContent();
        return Ok(result);
    }
}