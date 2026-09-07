using PaymentService.Domain.Entities;

namespace PaymentService.Domain.Contracts
{
    public interface IProcessedStripeEventRepository
    {
        Task<bool> ExistsAsync(string stripeEventId, CancellationToken cancellationToken = default);
        Task AddAsync(ProcessedStripeEvent stripeEvent, CancellationToken cancellationToken = default);
    }
}
