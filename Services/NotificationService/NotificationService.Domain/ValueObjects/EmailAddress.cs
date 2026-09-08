namespace NotificationService.Domain.ValueObjects
{
    public sealed record EmailAddress
    {
        public string Value { get; }
        public EmailAddress(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email is required.");

            Value = value.Trim();
        }
        public override string ToString() => Value;
    }
}
