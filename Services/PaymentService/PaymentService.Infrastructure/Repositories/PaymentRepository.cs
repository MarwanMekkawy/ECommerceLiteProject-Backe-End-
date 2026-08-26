using PaymentService.Domain.Contracts;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        public Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
