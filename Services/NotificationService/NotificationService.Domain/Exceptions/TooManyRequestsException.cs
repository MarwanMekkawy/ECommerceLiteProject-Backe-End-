namespace NotificationService.Domain.Exceptions
{
    public class TooManyRequestsException : Exception
    {
        public TooManyRequestsException(string message) : base(message) { }
    }
}
