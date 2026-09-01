using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Contracts;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enums;

namespace PaymentService.Infrastructure.Repositories
{
    public class PaymentRepository(PaymentDbContext _context) : IPaymentRepository
    {
        public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Payments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments.FirstOrDefaultAsync(x => x.OrderId == orderId, cancellationToken);
        }

        public async Task<Payment?> GetByStripePaymentIntentIdAsync(string paymentIntentId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments.FirstOrDefaultAsync(x=> x.StripePaymentIntentId == paymentIntentId, cancellationToken);
        }

        public async Task<Payment?> GetByStripeRefundIdAsync(string RefundId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments.FirstOrDefaultAsync(x => x.StripeRefundId == RefundId, cancellationToken);
        }

        public async Task<List<Payment>> GetSucceededPaymentsWithUnconfirmedOrderAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Payments.Where(x => x.Status == PaymentStatus.Succeeded && !x.IsOrderCompletionConfirmed).ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            await _context.Payments.AddAsync(payment, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
