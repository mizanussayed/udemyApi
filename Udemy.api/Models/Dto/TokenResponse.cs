using System.Text.Json.Serialization;

namespace Udemy.api.Models.Dto;

  public class TokenResponse
    {
        public required string Id { get; set; }
        public required string UserName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public List<string>? Roles { get; set; }
        public bool IsVerified { get; set; }
        public string? JWToken { get; set; }
        public DateTime IssuedOn { get; set; }
        public DateTime ExpiresOn { get; set; }

        [JsonIgnore]
        public string? RefreshToken { get; set; }
    }
