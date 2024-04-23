using Game_Store.Application.Abstractions;
using Game_Store.Domain.Entities;
using Game_Store.Domain.Entities.Auth;
using Game_Store.Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Game_Store.Infrastructure.Persistance
{
    public class NovaStoreDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IAppDbContext
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

            builder.ApplyConfiguration(new GameConfiguration());
            builder.ApplyConfiguration(new SystemRequirementConfiguration());

            builder.Entity<GameEdition>()
                .HasKey(ge => ge.Id);

            builder.Entity<GameEdition>()
                .HasOne(ge => ge.Game)
                .WithMany(g => g.GameEditions)
                .HasForeignKey(ge => ge.GameId);
        }
    }
}
