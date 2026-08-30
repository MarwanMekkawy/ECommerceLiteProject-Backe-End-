using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.Abstractions;
using PaymentService.Application.DTOs;

namespace PaymentService.API.Controllers
{
    /// <summary>
    /// Provides endpoints for creating payments and handling Stripe webhook events.
    /// </summary>
    [Route("api/v1/payments")]
    [ApiController]
    [Authorize]
    public class PaymentsController(IPaymentAppService paymentAppService) : ControllerBase
    {
        /// <summary>
        /// Creates a payment for an order using the provided payment details.
        /// </summary>
        /// <param name="request">The payment creation request containing the order identifier, user identifier, amount, and currency.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>The created payment details, including the payment identifier, Stripe client secret, and payment status.</returns>
        [HttpPost("create-internal")]
        [Authorize]

        public async Task<ActionResult<CreatePaymentResponseDto>> CreatePayment(CreatePaymentRequestDto request, CancellationToken cancellationToken)
        {
            var result = await paymentAppService.CreatePaymentAsync(request.OrderId, request.UserId, request.Amount, request.Currency, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Handles webhook events sent by Stripe and updates the corresponding payment.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>An empty successful response when the webhook has been processed.</returns>
        [HttpPost("webhook")]
        [Authorize]
        public async Task<IActionResult> StripeWebhook(CancellationToken cancellationToken)
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync(cancellationToken);

            await paymentAppService.HandleStripeWebhookAsync(json, Request.Headers["Stripe-Signature"].ToString(), cancellationToken);

            return Ok();
        }
    }
}