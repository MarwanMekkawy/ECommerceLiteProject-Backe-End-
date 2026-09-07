using PaymentService.Domain.Contracts;

namespace PaymentService.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PaymentDbContext _context;
        public IPaymentRepository Payments { get; }
        public IProcessedStripeEventRepository ProcessedStripeEvents { get; }

        public UnitOfWork(PaymentDbContext context, IPaymentRepository paymentrepo, IProcessedStripeEventRepository ProcessedStripeEventRepo)
        {
            _context = context;
            Payments = paymentrepo;
            ProcessedStripeEvents = ProcessedStripeEventRepo;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
