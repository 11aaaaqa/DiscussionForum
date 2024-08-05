using MimeKit;

namespace RegisterMicroservice.Api.Services
{
    public interface IEmailSender
    {
        Task SendEmail(MailboxAddress to, string subject, string content);
    }
}
