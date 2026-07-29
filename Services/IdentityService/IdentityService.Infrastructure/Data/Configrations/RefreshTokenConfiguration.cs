using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace IdentityService.Infrastructure.Data.Configrations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.TokenHash)
                .IsRequired()
                .HasMaxLength(512);

            builder.HasIndex(rt => rt.TokenHash)
                .IsUnique();

            builder.Property(rt => rt.ReplacedByTokenHash)
                .HasMaxLength(512);

            builder.Property(rt => rt.ExpiresAt)
                .IsRequired();

            builder.HasIndex(rt => rt.UserId);

            // Ignore computed properties (not strictly necessary, EF ignores
            // properties without setters/backing fields by convention if
            // they're not mapped, but explicit is safer for clarity)
            builder.Ignore(rt => rt.IsExpired);
            builder.Ignore(rt => rt.IsRevoked);
            builder.Ignore(rt => rt.IsActive);
        }
    }
}
