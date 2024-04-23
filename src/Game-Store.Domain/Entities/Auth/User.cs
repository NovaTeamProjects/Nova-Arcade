using Microsoft.AspNetCore.Identity;

namespace Game_Store.Domain.Entities.Auth
{
    public class User : IdentityUser<Guid>
    {
        public virtual List<Game> Games { get; set; }
        public virtual List<Game> Wishlist { get; set; }
        public virtual List<Game> Cart { get; set; }
    }
}
