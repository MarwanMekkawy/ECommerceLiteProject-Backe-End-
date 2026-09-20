using NotificationService.Application.Abstractions;
using NotificationService.Application.DTOs;
using NotificationService.Domain.Contracts;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using NotificationService.Domain.ValueObjects;
using System.Text.Json;

namespace NotificationService.Application.Services
{
    public class NotificationCreationService(IUnitOfWork uow) : INotificationCreationService
    {
        public async Task<Guid> CreateEmailNotificationAsync(SendEmailNotificationRequestDto request, CancellationToken cancellationToken)
        {
            var subject = request.Type switch
            {
                NotificationType.EmailConfirmation => "Email Confirmation",
                NotificationType.PasswordReset => "password Reset",
                NotificationType.EmailChangeConfirmation => "Email change Confirmation",
                NotificationType.EmailChanged => "Email address Was Changed",
                NotificationType.PasswordChanged => "Password Was Changed",
                NotificationType.OrderConfirmed => "Order confirmed",
                NotificationType.OrderCompleted => "Order completed",
                NotificationType.OrderCancelled => "Order cancelled",
                NotificationType.OrderExpired => "Order expired",
                NotificationType.PaymentFailed => "Payment failed",
                NotificationType.PaymentRefunded => "Payment refunded",
                _ => throw new ArgumentOutOfRangeException(nameof(request.Type))
            };

            var data = request.Data is null ? null : JsonSerializer.Serialize(request.Data);

            var notification = new Notification(request.UserId, new EmailAddress(request.RecipientEmail), request.Type, subject, data);

            await uow.NotificationRepo.AddAsync(notification, cancellationToken);

            await uow.SaveChangesAsync(cancellationToken);

            return notification.Id;
        }
    }
}