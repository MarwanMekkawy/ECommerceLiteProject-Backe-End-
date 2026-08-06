namespace OrderService.Domain.Exceptions.DomainExceptions
{
    public class OrderStateException : DomainException
    {
        public OrderStateException(string message) : base(message) { }
    }
}
