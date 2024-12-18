using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AuthVerif.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string otp);
    }

    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly string _sendGridApiKey;

        public EmailService(ILogger<EmailService> logger, string sendGridApiKey)
        {
            _logger = logger;
            _sendGridApiKey = sendGridApiKey;
        }

        public async Task SendEmailAsync(string toEmail, string otp)
        {
            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress("ikoto35@gmail.com", "Orbital Overlord");
            var to = new EmailAddress(toEmail);
            var subject = "Votre code vérification";
            var plainTextContent = $"Bonjour,\n\nVotre code de vérification est : {otp}\n\nSi vous n'avez pas demandé ce code, veuillez ignorer ce message.";
            var htmlContent = $@"<p>Bonjour,</p>
            <p>Votre code de vérification est : <strong>{otp}</strong></p>
            <p>Si vous n'avez pas demandé ce code, veuillez ignorer ce message.</p>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            try
            {
                var response = await client.SendEmailAsync(msg);
                _logger.LogInformation("Email envoyé avec succès à {ToEmail}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur s'est produite lors de l'envoi de l'email à {ToEmail}", toEmail);
                throw new InvalidOperationException("Une erreur s'est produite lors de l'envoi de l'email.", ex);
            }
        }

    }
}
