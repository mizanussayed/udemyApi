using Microsoft.AspNetCore.Identity;

namespace Udemy.api.Models.Domain;

public class ApplicationUser : IdentityUser
{
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public bool IsActive { get; set; } = false;
}
