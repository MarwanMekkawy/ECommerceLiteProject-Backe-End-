using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Data.Configrations
{
    public class ProcessedMailjetEventConfiguration : IEntityTypeConfiguration<ProcessedMailjetEvent>
    {
        public void Configure(EntityTypeBuilder<ProcessedMailjetEvent> builder)
        {
            builder.HasKey(x=>x.MailjetEventId);

            builder.Property(x => x.MailjetEventId)
                .IsRequired()
                .HasMaxLength(256);
        }
    }
}
