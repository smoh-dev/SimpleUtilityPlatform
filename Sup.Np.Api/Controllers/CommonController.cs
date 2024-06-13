using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sup.Common;
using Sup.Common.Entities.Redmine;
using Sup.Common.Kms;
using Sup.Common.Logger;
using Sup.Common.Models.RequestParams;
using Sup.Common.Models.Responses;
using Sup.Np.Api.Repositories.Database;
using Sup.Np.Api.Services.Product;

namespace Sup.Np.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CommonController(SupLog log, CommonService svc, IDbRepository db, AwsKmsEncryptor awsKmsEncryptor) : ControllerBase
{
    private readonly Stopwatch _sw = new();
    private readonly SupLog _log = log.ForContext<CommonController>();
    private readonly CommonService _svc = svc;
    private readonly IDbRepository _db = db;
    private readonly AwsKmsEncryptor _enc = awsKmsEncryptor;

    /// <summary>
    /// Ping the server to check if it is alive.
    /// </summary>
    /// <returns></returns>
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok();
    }
    
    #region KMSTest

    [HttpPost("encrypt")]
    [Authorize(Policy = Consts.Auth.PolicyKms)]
    public async Task<IActionResult> EncryptString(string input)
    {
        try
        {
            _sw.Restart();
            var encrypted = await _enc.EncryptStringAsync(input);
            _sw.Stop();
            _log.Verbose("{api_name} took {time}ms",
                nameof(EncryptString), _sw.ElapsedMilliseconds);

            return Ok(encrypted);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(false, Consts.ErrorCode.DatabaseError, ex.Message));
        
        }
    }
    
    [HttpPost("decrypt")]
    [Authorize(Policy = Consts.Auth.PolicyKms)]
    public async Task<IActionResult> DecryptString(string input)
    {
        try
        {
            _sw.Restart();
            var encrypted = await _enc.DecryptStringAsync(input);
            _sw.Stop();
            _log.Verbose("{api_name} took {time}ms",
                nameof(EncryptString), _sw.ElapsedMilliseconds);

            return Ok(encrypted);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(false, Consts.ErrorCode.DatabaseError, ex.Message));
        
        }
    }
    
    #endregion KMSTest
    

    #region Profile

    /// <summary>
    /// Get all profiles
    /// </summary>
    /// <returns></returns>
    [HttpGet("profiles")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GetProfilesResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    public async Task<IActionResult> GetProfiles()
    {
        try
        {
            _sw.Restart();
            var profiles = await _db.GetProfilesAsync<Profile>();
            _sw.Stop();
            _log.Verbose("{api_name} took {time}ms",
                nameof(GetProfiles), _sw.ElapsedMilliseconds);

            var result = new GetProfilesResponse(profiles);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(false, Consts.ErrorCode.DatabaseError, ex.Message));
        }
    }

    [HttpGet("profiles/es/{productCode}")]
    public async Task<IActionResult> GetEsConfigs(string productCode)
    {
        try
        {
            _sw.Restart();
            var esConfigs = await _svc.GetEsConfigsAsync(productCode);
            _sw.Stop();
            _log.Verbose("{api_name} took {time}ms",
                nameof(GetEsConfigs), _sw.ElapsedMilliseconds);

            return Ok(esConfigs);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(false, Consts.ErrorCode.DatabaseError, ex.Message));
        }
    }

    #endregion Profile
    
    #region License
    [HttpGet("license/{productCode}")]
    [Authorize(Policy = Consts.Auth.PolicyLicense)]
    public async Task<IActionResult> GenerateLicense(string productCode)
    {
        try
        {
            _sw.Restart();
            var license = await _svc.GenerateLicenseAsync(productCode);
            _sw.Stop();
            _log.Verbose("{api_name} took {time}ms",
                nameof(GenerateLicense), _sw.ElapsedMilliseconds);

            return Ok(license);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(false, Consts.ErrorCode.DatabaseError, ex.Message));
        }
    }
    [HttpPost("license")]
    public async Task<IActionResult> CheckLicense([FromBody]CheckLicenseParam param)
    {
        try
        {
            _sw.Restart();
            var license = await _svc.CheckLicenseAsync(param.ProductCode, param.HashedLicenseKey);
            _sw.Stop();
            _log.Verbose("{api_name} took {time}ms",
                nameof(CheckLicense), _sw.ElapsedMilliseconds);
            return (license != null) ? Ok(new CheckLicenseResponse
            {
                Audience = license.AuthAudience,
                SigningKey = license.AuthSigningKey,
            }) : Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(false, Consts.ErrorCode.DatabaseError, ex.Message));
        }
    }
    
    #endregion License
    
}