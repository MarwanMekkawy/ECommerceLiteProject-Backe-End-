namespace OrderService.Domain.Exceptions.DomainExceptions
{
    public class InvalidOrderException : DomainException
    {
        public InvalidOrderException(string message) : base(message) { }
    }
}
