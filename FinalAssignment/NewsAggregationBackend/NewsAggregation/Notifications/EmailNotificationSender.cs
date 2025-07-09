using NewsAggregation.Notifications.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Notifications
{
    public class EmailNotificationSender : INotificationSender
    {
        private readonly IEmailService _emailService;
        public EmailNotificationSender(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendAsync(string to, string subject, string message)
        {
            await _emailService.SendEmailAsync(to, subject, message, true);
        }
    }

}
