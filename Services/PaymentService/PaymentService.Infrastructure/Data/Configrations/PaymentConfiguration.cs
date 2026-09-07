using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Data.Configrations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderId)
                .IsRequired();

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(x => x.IsOrderCompletionConfirmed)
                .IsRequired();

            builder.Property(x => x.IsOrderCancellationDueToRefundConfirmed)
                .IsRequired();

            builder.Property(x => x.OrderCompletionReattempts)
                .IsRequired();

            builder.Property(x => x.NextOrderCompletionAttemptAt);

            builder.Property(x => x.OrderCancellationReattempts)
                .IsRequired();

            builder.Property(x => x.NextOrderCancellationAttemptAt);

            builder.Property(x => x.StripePaymentIntentId)
                .HasMaxLength(100);

            builder.Property(x => x.StripeRefundId)
                .HasMaxLength(100);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.SucceededAt);

            builder.Property(x => x.PaymentFailedAt);

            builder.Property(x => x.RefundInitiatedAt);

            builder.Property(x => x.RefundedAt);

            builder.Property(x => x.RefundFailedAt);

            builder.Property(x => x.PaymentFailureReason)
                .HasMaxLength(500);

            builder.Property(x => x.RefundFailureReason)
                .HasMaxLength(500);

            builder.OwnsOne(x => x.Amount, money =>
            {
                money.Property(x => x.Amount)
                    .HasColumnName("Amount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                money.Property(x => x.Currency)
                    .HasColumnName("Currency")
                    .HasConversion<string>()
                    .IsRequired();
            });
        }
    }
}
