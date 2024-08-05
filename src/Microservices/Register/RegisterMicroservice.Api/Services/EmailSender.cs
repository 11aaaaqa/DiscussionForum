using MailKit.Net.Smtp;
using MimeKit;

namespace RegisterMicroservice.Api.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration config;

        public EmailSender(IConfiguration config)
        {
            this.config = config;
        }
        public async Task SendEmail(MailboxAddress to, string subject, string content)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Подтверждение почты", config["Email:From"]));
            emailMessage.To.Add(to);
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = content };

            using var client = new SmtpClient();
            await client.ConnectAsync(config["Email:Smtp"], int.Parse(config["Email:Port"]), true);
            await client.AuthenticateAsync(config["Email:UserName"], config["Email:Password"]);
            await client.SendAsync(emailMessage);

            await client.DisconnectAsync(true);
        }
    }
}
