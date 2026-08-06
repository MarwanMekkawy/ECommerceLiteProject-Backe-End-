using OrderService.Domain.Shared.Enums;


namespace OrderService.Domain.Shared.ValueObject
{
    public sealed record Money
    {
        public decimal Amount { get; }
        public CurrencyCode Currency { get; }

        private Money() { }

        public Money(decimal amount, CurrencyCode currency)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative.");

            Amount = amount;
            Currency = currency;
        }
    }
}
