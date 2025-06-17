namespace DressCode.Models
{
    using MailKit.Net.Smtp;
    using MimeKit;
    using Microsoft.Extensions.Options;
    using Microsoft.AspNetCore.Identity;

    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendEmailToLoyalUsersAsync(string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly UserManager<Korisnik> _userManager;

        public EmailService(IOptions<EmailSettings> emailSettings, UserManager<Korisnik> userManager)
        {
            _emailSettings = emailSettings.Value;
            _userManager = userManager;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendEmailToLoyalUsersAsync(string subject, string body)
        {
            var loyalUsers = _userManager.Users
                .Where(u => u.IsLoyal && !string.IsNullOrEmpty(u.Email))
                .ToList();

            foreach (var user in loyalUsers)
            {
                await SendEmailAsync(user.Email, subject, body);
                // await Task.Delay(200); // ako želiš throttling zbog SMTP limita
            }
        }
    }

}