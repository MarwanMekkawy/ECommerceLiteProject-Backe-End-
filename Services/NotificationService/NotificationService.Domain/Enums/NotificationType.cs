namespace NotificationService.Domain.Enums
{
    public enum NotificationType
    {
        // Identity
        EmailConfirmation,
        PasswordReset,
        EmailChanged,
        PasswordChanged,

        // Orders
        OrderConfirmed,
        OrderCompleted,
        OrderCancelled,
        OrderExpired,

        // Payments
        PaymentSucceeded,
        PaymentFailed,
        PaymentRefunded
    }
}
