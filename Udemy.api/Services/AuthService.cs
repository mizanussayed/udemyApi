using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Udemy.api.Exceptions;
using Udemy.api.Models.Domain;
using Udemy.api.Models.Dto;
using Udemy.api.Settings;

namespace Udemy.api.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JWTSettings _jwtSettings;
    private readonly IMailService _mailService;

    public AuthService(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<JWTSettings> jwtSettings,
        IMailService mailService,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtSettings = jwtSettings.Value;
        _signInManager = signInManager;
        _mailService = mailService;
    }

    public async Task<TokenResponse> GetTokenAsync(TokenRequest request, string ipAddress)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        Throw.Exception.IfNull(user, nameof(user), $"No Accounts Registered with {request.Email}.");
        var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
        Throw.Exception.IfFalse(user.EmailConfirmed, $"Email is not confirmed for '{request.Email}'.");
        Throw.Exception.IfFalse(user.IsActive, $"Account for '{request.Email}' is not active. Please contact the Administrator.");
        Throw.Exception.IfFalse(result.Succeeded, $"Invalid Credentials for '{request.Email}'.");
        JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user, ipAddress);
        var response = new TokenResponse
        {
            Id = user.Id,
            JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            IssuedOn = jwtSecurityToken.ValidFrom.ToLocalTime(),
            ExpiresOn = jwtSecurityToken.ValidTo.ToLocalTime(),
            Email = user.Email,
            UserName = user.UserName
        };
        var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        response.Roles = rolesList.ToList();
        response.IsVerified = user.EmailConfirmed;
        var refreshToken = GenerateRefreshToken(ipAddress);
        response.RefreshToken = refreshToken.Token;
        return response;
    }

    private async Task<JwtSecurityToken> GenerateJWToken(ApplicationUser user, string ipAddress)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();
        for (int i = 0; i < roles.Count; i++)
        {
            roleClaims.Add(new Claim("roles", roles[i]));
        }
        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
                new Claim("first_name", user.FirstName),
                new Claim("last_name", user.LastName),
                new Claim("full_name", $"{user.FirstName} {user.LastName}"),
                new Claim("ip", ipAddress)
            }
        .Union(userClaims)
        .Union(roleClaims);
        return JWTGeneration(claims);
    }

    private JwtSecurityToken JWTGeneration(IEnumerable<Claim> claims)
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
            signingCredentials: signingCredentials);
        return jwtSecurityToken;
    }

    private static string RandomTokenString()
    {
        using var rngCryptoServiceProvider = new RSACryptoServiceProvider();
        var randomBytes = new byte[40];
        // rngCryptoServiceProvider.(randomBytes);
        return BitConverter.ToString(randomBytes).Replace("-", "");
    }

    private static RefreshToken GenerateRefreshToken(string ipAddress)
    {
        return new RefreshToken
        {
            Token = RandomTokenString(),
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };
    }

    public async Task<string> RegisterAsync(RegisterRequest request, string? origin)
    {
        var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
        if (userWithSameUserName != null)
            throw new ApiException($"Username '{request.UserName}' is already taken.");

        var user = new ApplicationUser
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName
        };
        var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
        if (userWithSameEmail == null)
        {
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Basic");
                var verificationUri = await SendVerificationEmail(user, origin);
                return string.Concat($"message: User Registered. Confirmation Mail has been delivered to your Mailbox. (DEV) Please confirm your account by visiting this URL {verificationUri}");
            }
            else
            {
                throw new ApiException($"{result.Errors}");
            }
        }
        else
        {
            throw new ApiException($"Email {request.Email} is already registered.");
        }
    }

    private async Task<string> SendVerificationEmail(ApplicationUser user, string? origin)
    {
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var route = "api/auth/confirm-email/";
        var _enpointUri = new Uri(string.Concat($"{origin}/", route));
        var verificationUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "userId", user.Id);
        verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
        await _mailService.SendAsync(new MailRequest
        {
            To = user.Email,
            Body = $"Please confirm your account by <a href='{verificationUri}'>clicking here</a>.",
            Subject = "Confirm Registration"
        }
        );
        return verificationUri;
    }

    public async Task<string> ConfirmEmailAsync(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);
        user.IsActive = true;
        await  _userManager.UpdateAsync(user);
        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
            return string.Concat($" message: Account Confirmed for {user.Email}. You can now use the /api/identity/token endpoint to generate JWT.");
        throw new ApiException($"An error occured while confirming {user.Email}.");
    }

    public async Task<string> ForgotPassword(ForgotPasswordRequest model, string? origin)
    {
        var account = await _userManager.FindByEmailAsync(model.Email);

        // always return ok response to prevent email enumeration
        if (account == null) return "Email Is Not Valid";

        var code = await _userManager.GeneratePasswordResetTokenAsync(account);
        var route = "api/auth/reset-password/";
        var _enpointUri = new Uri(string.Concat($"{origin}/", route));
        var emailRequest = new MailRequest()
        {
            To = model.Email,
            Subject = "Reset Password",
            Body = $"You reset token is - {code} <br/> and reset password using this endpoint ---> {_enpointUri}",
        };
        await _mailService.SendAsync(emailRequest);
        return "Check Your Email Address Reset Token has Sent.";
    }

    public async Task<string> ResetPassword(ResetPasswordRequest model)
    {
        var account = await _userManager.FindByEmailAsync(model.Email) ?? throw new ApiException($"No Accounts Registered with {model.Email}.");
        var result = await _userManager.ResetPasswordAsync(account, model.Token, model.Password);
        if (result.Succeeded)
            return "Password Reset Done.";
        throw new ApiException($"Error occured while reseting the password.");
    }
}