using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Contracts;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Repositories
{
    public class ProcessedStripeEventRepository(PaymentDbContext _context) : IProcessedStripeEventRepository
    {
        public async Task<bool> ExistsAsync(string stripeEventId, CancellationToken cancellationToken = default)
        {
            return await _context.ProcessedStripeEvents.AnyAsync(x => x.StripeEventId == stripeEventId, cancellationToken);
        }

        public async Task AddAsync(ProcessedStripeEvent stripeEvent, CancellationToken cancellationToken = default)
        {
            await _context.ProcessedStripeEvents.AddAsync(stripeEvent, cancellationToken);
        }
    }
}
