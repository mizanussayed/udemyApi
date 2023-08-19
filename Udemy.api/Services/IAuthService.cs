using Udemy.api.Models.Dto;

namespace Udemy.api.Services;

public interface IAuthService
{
    Task<TokenResponse> GetTokenAsync(TokenRequest request, string ipAddress);

    Task<string> RegisterAsync(RegisterRequest request, string? origin);

    Task<string> ConfirmEmailAsync(string userId, string code);

    Task <string> ForgotPassword(ForgotPasswordRequest model, string? origin);

    Task<string> ResetPassword(ResetPasswordRequest model);
}
