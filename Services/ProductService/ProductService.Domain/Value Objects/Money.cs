using ProductService.Domain.Enums;


namespace ProductService.Domain.Value_Objects
{
    public record Money
    {
        public decimal Amount { get; init; }
        public CurrencyCode Currency { get; init; }

        public Money(decimal amount, CurrencyCode currency)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative");

            Amount = amount;
            Currency = currency;
        }

        public Money ApplyDiscount(decimal percentage)
        {
            if (percentage is < 0 or > 100)
                throw new ArgumentException("Invalid discount percentage");
            return this with { Amount = Amount - (Amount * percentage / 100) };
        }
    }
}
