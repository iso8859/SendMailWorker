using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SendMailFunction.Configuration;
using SendMailFunction.Models;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace SendMailFunction.Services;

public interface IEmailService
{
    Task<(bool Success, string? MessageId, string? Error)> SendEmailAsync(EmailRequest emailRequest);
}

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
        _smtpSettings = EnvironmentHelper.GetSmtpSettings();
    }

    public async Task<(bool Success, string? MessageId, string? Error)> SendEmailAsync(EmailRequest emailRequest)
    {
        try
        {
            // Validate SMTP configuration
            if (string.IsNullOrEmpty(_smtpSettings.Host) || 
                string.IsNullOrEmpty(_smtpSettings.Username) || 
                string.IsNullOrEmpty(_smtpSettings.FromEmail))
            {
                return (false, null, "SMTP configuration is incomplete. Please check environment variables.");
            }

            // Validate email format
            if (!IsValidEmail(emailRequest.To))
            {
                return (false, null, "Invalid email format.");
            }

            // Load and process template
            var htmlContent = await ProcessTemplateAsync(emailRequest);

            // Create email message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromEmail));
            message.To.Add(new MailboxAddress("", emailRequest.To));
            message.Subject = emailRequest.Subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlContent
            };
            message.Body = bodyBuilder.ToMessageBody();

            // Generate message ID
            var messageId = $"<{Guid.NewGuid()}@{_smtpSettings.Host}>";
            message.MessageId = messageId;

            // Send email
            using var client = new SmtpClient();
            
            _logger.LogInformation("Connecting to SMTP server: {Host}:{Port}", _smtpSettings.Host, _smtpSettings.Port);
            
            await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, 
                _smtpSettings.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);

            // Authenticate
            if (_smtpSettings.AuthType.Equals("oauth2", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(_smtpSettings.OAuth2AccessToken))
                {
                    return (false, null, "OAuth2 access token is required for OAuth2 authentication.");
                }

                // OAuth2 authentication for Gmail
                var oauth2 = new SaslMechanismOAuth2(_smtpSettings.Username, _smtpSettings.OAuth2AccessToken);
                await client.AuthenticateAsync(oauth2);
            }
            else
            {
                // Basic authentication
                if (string.IsNullOrEmpty(_smtpSettings.Password))
                {
                    return (false, null, "SMTP password is required for basic authentication.");
                }
                
                await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {To} with message ID {MessageId}", 
                emailRequest.To, messageId);

            return (true, messageId, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", emailRequest.To);
            return (false, null, ex.Message);
        }
    }

    private async Task<string> ProcessTemplateAsync(EmailRequest emailRequest)
    {
        try
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "template.html");
            
            if (!File.Exists(templatePath))
            {
                _logger.LogWarning("Template file not found at {Path}, using basic template", templatePath);
                return CreateBasicTemplate(emailRequest);
            }

            var template = await File.ReadAllTextAsync(templatePath);
            
            // Replace template variables
            template = template.Replace("{{subject}}", emailRequest.Subject);
            template = template.Replace("{{body}}", emailRequest.Body);
            
            return template;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing template, using basic template");
            return CreateBasicTemplate(emailRequest);
        }
    }

    private static string CreateBasicTemplate(EmailRequest emailRequest)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>{emailRequest.Subject}</title>
</head>
<body>
    <h2>{emailRequest.Subject}</h2>
    <div>{emailRequest.Body}</div>
    <hr>
    <p><small>Sent via Azure Function</small></p>
</body>
</html>";
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var emailRegex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.IgnoreCase);
            return emailRegex.IsMatch(email);
        }
        catch
        {
            return false;
        }
    }
}