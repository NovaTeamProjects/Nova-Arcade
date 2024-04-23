using Game_Store.Application.Abstractions;
using Game_Store.Domain.Entities;
using Game_Store.Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Game_Store.Infrastructure.Persistance
{
    public class NovaStoreDbContext : IdentityDbContext<User, Role, Guid>, IAppDbContext
    {
        public NovaStoreDbContext(DbContextOptions<NovaStoreDbContext> options)
            : base(options)
            => Database.Migrate();

        public DbSet<Game> Games { get; set; }
        public DbSet<SystemRequirement> SystemRequirements { get; set; }

        async ValueTask<int> IAppDbContext.SaveChangesAsync(CancellationToken cancellationToken)
            => await base.SaveChangesAsync(cancellationToken);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
