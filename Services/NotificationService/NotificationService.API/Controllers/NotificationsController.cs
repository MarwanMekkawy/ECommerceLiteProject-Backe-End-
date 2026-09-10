using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Abstractions;
using NotificationService.Application.DTOs;

namespace NotificationService.API.Controllers
{
    [Route("api/v1/notifications")]
    [ApiController]
    public class NotificationsController(INotificationCreationService NotificationCreationService, IMailjetWebhookService webhookService) : ControllerBase
    {
        /// <summary>
        /// Creates an email notification for a user.
        /// </summary>
        /// <param name="request">The notification details.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>The ID of the created notification.</returns>
        [HttpPost("internal")]
        [Authorize(AuthenticationSchemes = "ServiceJwt")]
        public async Task<IActionResult> CreateEmailNotification(SendEmailNotificationRequestDto request, CancellationToken cancellationToken)
        {
            var notificationId = await NotificationCreationService.CreateEmailNotificationAsync(request, cancellationToken);

            return Ok(notificationId);
        }

        /// <summary>
        /// Handles webhook events received from Mailjet.
        /// </summary>
        /// <param name="request">The Mailjet webhook event.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> HandleWebhook(MailjetWebhookRequest request, CancellationToken cancellationToken)
        {
            await webhookService.ProcessEventAsync(request, cancellationToken);

            return Ok();
        }
    }
}