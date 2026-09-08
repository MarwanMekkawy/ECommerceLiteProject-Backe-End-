namespace NotificationService.Domain.Entities
{
    public class ProcessedMailjetEvent
    {
        public string MailjetEventId { get; private set; } = null!;

        public ProcessedMailjetEvent() { }

        public ProcessedMailjetEvent(string mailjetEventId)
        {
            MailjetEventId = mailjetEventId;
        }
    }
}
