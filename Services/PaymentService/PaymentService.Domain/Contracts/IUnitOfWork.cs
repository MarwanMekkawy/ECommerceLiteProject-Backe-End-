namespace PaymentService.Domain.Contracts
{
    public interface IUnitOfWork
    {
        public IPaymentRepository Payments { get; }
        public IProcessedStripeEventRepository ProcessedStripeEvents { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
