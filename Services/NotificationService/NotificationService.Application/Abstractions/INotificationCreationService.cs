using NotificationService.Application.DTOs;

namespace NotificationService.Application.Abstractions
{
    public interface INotificationCreationService
    {
        Task<Guid> CreateEmailNotificationAsync(SendEmailNotificationRequestDto request, CancellationToken cancellationToken);
    }
}