using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.Domain.Entities;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Infrastructure.Data.Configrations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.RecipientEmail)
                .HasConversion(email => email.Value, value => new EmailAddress(value))
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.Type)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(x => x.Subject)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.AttemptCount)
                .IsRequired();

            builder.Property(x => x.LastAttemptAt);

            builder.Property(x => x.SentAt);

            builder.Property(x => x.FailureReason)
                .HasMaxLength(2000);

            builder.Property(x => x.CreatedAt)
                .IsRequired();
        }
    }
}
