using Microsoft.AspNetCore.Authentication;

namespace Game_Store.Models
{
    public class LoginVM
    {
        public string? ReturnUrl { get; set; }
        // AuthenticationScheme is in Microsoft.AspNetCore.Authentication namespace
        public IList<AuthenticationScheme>? ExternalLogins { get; set; }
    }
}
