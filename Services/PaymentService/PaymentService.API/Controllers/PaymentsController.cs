using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.Abstractions;
using PaymentService.Application.DTOs;
using System.Security.Claims;

namespace PaymentService.API.Controllers
{
    /// <summary>
    /// Provides endpoints for creating payments and handling Stripe webhook events.
    /// </summary>
    [Route("api/v1/payments")]
    [ApiController]
    public class PaymentsController(IPaymentAppService paymentAppService) : ControllerBase
    {
        /// <summary>
        /// Creates a payment for an order using the provided payment details.
        /// </summary>
        /// <param name="request">The payment creation request containing the order identifier, user identifier, amount, and currency.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>The created payment details, including the payment identifier, Stripe client secret, and payment status.</returns>
        [HttpPost("create-internal")]
        [Authorize(AuthenticationSchemes = "ServiceJwt")]
        public async Task<ActionResult<CreatePaymentResponseDto>> CreatePayment(CreatePaymentRequestDto request, CancellationToken cancellationToken)
        {
            var result = await paymentAppService.CreatePaymentAsync(request.OrderId, request.UserId, request.Amount, request.Currency, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Retries a failed payment by confirming the existing Stripe PaymentIntent associated with the payment.
        /// </summary>
        /// <param name="paymentId">The identifier of the payment to retry.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>The payment details, including the payment identifier, Stripe client secret, and current payment status.</returns>
        /// <remarks>
        /// The payment must belong to the authenticated user and have a status of <see cref="PaymentStatus.Failed"/>.
        /// The existing Stripe PaymentIntent is reused rather than creating a new PaymentIntent.
        /// The retry may succeed, fail again, require additional customer action, or enter processing.
        /// The final payment state is handled through Stripe webhook events.
        /// </remarks>
        [HttpPost("{paymentId}/retry")]
        [Authorize]
        public async Task<ActionResult<CreatePaymentResponseDto>> RetryPayment(Guid paymentId, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await paymentAppService.RetryPaymentAsync(paymentId, userId, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Handles webhook events sent by Stripe and updates the corresponding payment.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>An empty successful response when the webhook has been processed.</returns>
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook(CancellationToken cancellationToken)
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync(cancellationToken);

            await paymentAppService.HandleStripeWebhookAsync(json, Request.Headers["Stripe-Signature"].ToString(), cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Initiates a refund for a payment on behalf of an administrator.
        /// </summary>
        /// <param name="paymentId">The identifier of the payment to refund.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>An empty successful response when the refund has been successfully initiated.</returns>
        /// <remarks>
        /// The endpoint is restricted to administrators.
        /// The payment is first marked as <see cref="PaymentStatus.RefundInitiated"/> and saved before the refund request is sent to Stripe.
        /// Stripe then processes the refund asynchronously and sends webhook events that update the payment to its final refund status.
        /// </remarks>
        [HttpPost("admin/{paymentId}/refund")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RefundPayment(Guid paymentId, CancellationToken cancellationToken)
        {
            await paymentAppService.RefundPaymentAsync(paymentId, cancellationToken);

            return NoContent();
        }
    }
}