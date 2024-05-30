using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sup.Mm.Api.Services;
using Sup.Mm.Common.DTO;

namespace Sup.Mm.Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class NoteController : ControllerBase
{
    private readonly Stopwatch _sw = new();
    private readonly NoteService _svc;
    public NoteController(NoteService svc)
    {
        _svc = svc;
    }

    [HttpGet("notes")]
    [Produces("application/json")]
    public async Task<IActionResult> GetNotesAsync()
    {
        try
        {
            _sw.Restart();
            var result = await _svc.GetNotesAsync();

            if (result.Count == 0)
                return NotFound();
            else
                return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
        finally
        {
            _sw.Stop();
        }

        
    }
}