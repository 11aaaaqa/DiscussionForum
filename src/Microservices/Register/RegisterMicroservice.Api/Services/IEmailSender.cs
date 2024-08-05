using MimeKit;

namespace RegisterMicroservice.Api.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(MailboxAddress to, string subject, string content);
    }
}
