namespace PaymentService.Domain.Entities
{
    public class ProcessedStripeEvent
    {
        public string StripeEventId { get; private set; } = null!;

        private ProcessedStripeEvent() { }

        public ProcessedStripeEvent(string stripeEventId)
        {
            StripeEventId = stripeEventId;
        }
    }
}
