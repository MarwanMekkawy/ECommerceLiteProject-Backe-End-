using NotificationService.Application.Abstractions;
using NotificationService.Application.DTOs;
using NotificationService.Domain.Contracts;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Services
{
    public class MailjetWebhookService(IUnitOfWork uow) : IMailjetWebhookService
    {
        public async Task ProcessEventAsync(MailjetWebhookRequest request, CancellationToken cancellationToken)
        {
            var mailjetEventId = $"{request.MessageID}:{request.Event}";

            if (await uow.ProcessedMailjetEventRepo.ExistsAsync(mailjetEventId, cancellationToken))
                return;

            if (!Guid.TryParse(request.CustomID, out var notificationId))
                return;

            var notification = await uow.NotificationRepo.GetByIdAsync(notificationId, cancellationToken);

            if (notification is null)
                return;

            switch (request.Event)
            {
                case "sent":
                    notification.MarkAsSent();
                    break;

                case "bounce":
                case "blocked":
                case "spam":
                    notification.MarkAsFailed(request.Error ?? $"Mailjet event: {request.Event}");
                    break;
            }

            await uow.ProcessedMailjetEventRepo.AddAsync(new ProcessedMailjetEvent(mailjetEventId), cancellationToken);

            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}