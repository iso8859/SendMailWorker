using Microsoft.Extensions.Configuration;

namespace SendMailFunction.Configuration;

public static class ContactFormSettings
{
    public static string GetRecipientEmail(IConfiguration configuration)
    {
        // Try to get recipient email from environment variables
        return configuration["CONTACT_RECIPIENT_EMAIL"] 
               ?? configuration["FROM_EMAIL"] 
               ?? "contact@yourcompany.com";
    }
}