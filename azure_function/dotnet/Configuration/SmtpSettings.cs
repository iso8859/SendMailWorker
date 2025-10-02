namespace SendMailFunction.Configuration;

public class SmtpSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool UseSsl { get; set; } = true;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string AuthType { get; set; } = "basic";
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    
    // OAuth2 settings for Gmail
    public string? OAuth2ClientId { get; set; }
    public string? OAuth2ClientSecret { get; set; }
    public string? OAuth2RefreshToken { get; set; }
    public string? OAuth2AccessToken { get; set; }
}

public static class EnvironmentHelper
{
    public static SmtpSettings GetSmtpSettings()
    {
        return new SmtpSettings
        {
            Host = Environment.GetEnvironmentVariable("SMTP_HOST") ?? string.Empty,
            Port = int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out var port) ? port : 587,
            UseSsl = bool.TryParse(Environment.GetEnvironmentVariable("SMTP_USE_SSL"), out var useSsl) ? useSsl : true,
            Username = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? string.Empty,
            Password = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? string.Empty,
            AuthType = Environment.GetEnvironmentVariable("AUTH_TYPE") ?? "basic",
            FromEmail = Environment.GetEnvironmentVariable("FROM_EMAIL") ?? string.Empty,
            FromName = Environment.GetEnvironmentVariable("FROM_NAME") ?? string.Empty,
            OAuth2ClientId = Environment.GetEnvironmentVariable("OAUTH2_CLIENT_ID"),
            OAuth2ClientSecret = Environment.GetEnvironmentVariable("OAUTH2_CLIENT_SECRET"),
            OAuth2RefreshToken = Environment.GetEnvironmentVariable("OAUTH2_REFRESH_TOKEN"),
            OAuth2AccessToken = Environment.GetEnvironmentVariable("OAUTH2_ACCESS_TOKEN")
        };
    }
}