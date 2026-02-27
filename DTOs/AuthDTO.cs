using System.ComponentModel.DataAnnotations;

namespace api_netcore.DTOs
{
    public class AuthDTO
    {
        public class LoginRequest
        {
            [Required]
            public string Username { get; set; } = null!;

            [Required]
            public string Password { get; set; } = null!;
        }

        public class AuthResponse
        {
            public string Token { get; set; } = null!;
            public DateTime ExpiresAt { get; set; }
        }
    }
}
