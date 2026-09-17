namespace NotificationService.Domain.Enums
{
    public enum NotificationType
    {
        // Identity
        EmailConfirmation,
        PasswordReset,
        EmailChangeConfirmation,
        EmailChanged,
        PasswordChanged,

        // Orders
        OrderConfirmed,
        OrderCompleted,
        OrderCancelled,
        OrderExpired,

        // Payments
        PaymentFailed,
        PaymentRefunded
    }
}
