using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace IdentityService.Infrastructure.Data.Configrations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired();

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(u => u.IsActive)
                .HasDefaultValue(true);

            builder.Property(u => u.IsEmailConfirmed)
                .HasDefaultValue(false);

            builder.Property(u => u.CreatedAt)
                .IsRequired();

            // Relationships
            builder.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.PasswordResetTokens)
                .WithOne(prt => prt.User)
                .HasForeignKey(prt => prt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.EmailVerificationTokens)
                .WithOne(evt => evt.User)
                .HasForeignKey(evt => evt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.EmailChangeTokens)
                .WithOne(ect => ect.User)
                .HasForeignKey(ect => ect.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.UserPasswordHistory)
                .WithOne(uph => uph.User)
                .HasForeignKey(uph => uph.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(u => u.IsActive);
        }
    }
}