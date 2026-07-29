using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace IdentityService.Infrastructure.Data.Configrations
{
    public class UserPasswordHistoryConfiguration : IEntityTypeConfiguration<UserPasswordHistory>
    {
        public void Configure(EntityTypeBuilder<UserPasswordHistory> builder)
        {
            builder.ToTable("UserPasswordHistories");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.PasswordHash)
                .IsRequired();

            builder.HasIndex(h => h.UserId);
        }
    }
}
