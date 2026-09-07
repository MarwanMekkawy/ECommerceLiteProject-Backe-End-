using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Data.Configrations
{
    public class ProcessedStripeEventConfigration : IEntityTypeConfiguration<ProcessedStripeEvent>
    {       
        public void Configure(EntityTypeBuilder<ProcessedStripeEvent> builder)
        {
            builder.HasKey(x => x.StripeEventId);

            builder.Property(x => x.StripeEventId)
                .IsRequired()
                .HasMaxLength(255);
        }
    }
}
