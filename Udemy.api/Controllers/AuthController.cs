using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Udemy.api.Models.Dto;
using Udemy.api.Services;

namespace Udemy.api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Generates a JSON Web Token for a valid combination of emailId and password.
    /// </summary>
    /// <param name="tokenRequest"></param>
    /// <returns></returns>
    [HttpPost("GetToken")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTokenAsync(TokenRequest tokenRequest)
    {
        var ipAddress = GenerateIPAddress();
        var token = await _authService.GetTokenAsync(tokenRequest, ipAddress);
        return Ok(token);
    }

    [HttpPost("register")]
    [ValidateModel]
    public async Task<IActionResult> RegisterAsync(RegisterRequest request)
    {
        var origin = Request.Headers["origin"];
        return Ok(await _authService.RegisterAsync(request, origin));
    }

    [HttpGet("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string code)
    {
        return Ok(await _authService.ConfirmEmailAsync(userId, code));
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
    {
        return Ok( await _authService.ForgotPassword(model, Request.Headers["origin"]));
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
    {
        return Ok(await _authService.ResetPassword(model));
    }

    private string GenerateIPAddress()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"];
        return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
    }
}