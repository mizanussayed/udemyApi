using System.ComponentModel.DataAnnotations;

namespace Udemy.api.Models.Dto;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
}
