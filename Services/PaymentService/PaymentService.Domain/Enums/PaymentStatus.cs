namespace PaymentService.Domain.Enums
{
    public enum PaymentStatus
    {
        Pending,
        RequiresAction,
        Processing,
        Succeeded,
        Failed,
        Cancelled,
        Refunding,
        Refunded
    }
}
