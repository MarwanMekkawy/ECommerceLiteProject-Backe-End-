using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace IdentityService.Infrastructure.Data.Configrations
{
    public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
    {
        public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
        {
            builder.ToTable("PasswordResetTokens");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.TokenHash)
                .IsRequired()
                .HasMaxLength(512);

            builder.HasIndex(t => t.TokenHash)
                .IsUnique();

            builder.Property(t => t.ExpiresAt)
                .IsRequired();

            builder.HasIndex(t => t.UserId);

            builder.Ignore(t => t.IsExpired);
            builder.Ignore(t => t.IsUsed);
            builder.Ignore(t => t.IsActive);

            builder.HasQueryFilter(t => t.User.IsActive);
        }
    }
}
