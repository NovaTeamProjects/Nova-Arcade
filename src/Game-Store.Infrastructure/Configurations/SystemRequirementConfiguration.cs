using Game_Store.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Game_Store.Infrastructure.Configurations
{
    public class SystemRequirementConfiguration : IEntityTypeConfiguration<SystemRequirement>
    {
        public void Configure(EntityTypeBuilder<SystemRequirement> builder)
        {
            builder.Property(x => x.OS)
                    .IsRequired();

            builder.Property(x => x.CPU)
                .IsRequired();

            builder.Property(x => x.RAM)
                    .HasMaxLength(10)
                        .IsRequired();

            builder.Property(x => x.DirectX)
                .HasMaxLength(15)
                    .IsRequired();

            builder.Property(x => x.DiskSpace)
                .HasMaxLength(10)
                    .IsRequired();

            builder.Property(x => x.LanguagesSupported)
                .IsRequired();
        }
    }
}
