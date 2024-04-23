using Game_Store.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Game_Store.Application.Abstractions
{
    public interface IAppDbContext
    {
        DbSet<Game> Games { get; set; }
        DbSet<SystemRequirement> SystemRequirements { get; set; }

        public ValueTask<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
