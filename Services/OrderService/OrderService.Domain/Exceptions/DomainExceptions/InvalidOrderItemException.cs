namespace OrderService.Domain.Exceptions.DomainExceptions
{
    public class InvalidOrderItemException : DomainException
    {
        public InvalidOrderItemException(string message) : base(message) { }
    }
}
