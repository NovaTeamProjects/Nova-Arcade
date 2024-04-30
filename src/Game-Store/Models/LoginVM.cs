using Microsoft.AspNetCore.Authentication;

namespace Game_Store.Models
{
    public class LoginVM
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? ReturnUrl { get; set; }
        public IList<AuthenticationScheme>? ExternalLogins { get; set; }
    }
}
