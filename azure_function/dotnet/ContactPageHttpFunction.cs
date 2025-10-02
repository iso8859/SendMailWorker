using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendMailFunction.Configuration;
using System.Net;
using System.Text;

namespace SendMailFunction;

public class ContactPageHttpFunction
{
    private readonly ILogger<ContactPageHttpFunction> _logger;
    private readonly IConfiguration _configuration;

    public ContactPageHttpFunction(ILogger<ContactPageHttpFunction> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [Function("ContactPage")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        _logger.LogInformation("ContactPage function triggered");

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/html; charset=utf-8");
        response.Headers.Add("Cache-Control", "no-cache");

        var recipientEmail = ContactFormSettings.GetRecipientEmail(_configuration);
        var html = GetContactPageHtml(recipientEmail);
        await response.WriteStringAsync(html, Encoding.UTF8);

        return response;
    }

    private static string GetContactPageHtml(string recipientEmail)
    {
        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Contact Us</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}

        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 20px;
        }}

        .container {{
            background: white;
            border-radius: 20px;
            box-shadow: 0 15px 35px rgba(0, 0, 0, 0.1);
            overflow: hidden;
            max-width: 500px;
            width: 100%;
        }}

        .header {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 40px 30px;
            text-align: center;
        }}

        .header h1 {{
            font-size: 2.5rem;
            margin-bottom: 10px;
            font-weight: 300;
        }}

        .header p {{
            opacity: 0.9;
            font-size: 1.1rem;
        }}

        .form-container {{
            padding: 40px 30px;
        }}

        .form-group {{
            margin-bottom: 25px;
        }}

        .form-group label {{
            display: block;
            margin-bottom: 8px;
            color: #555;
            font-weight: 500;
            font-size: 0.95rem;
        }}

        .form-group input,
        .form-group textarea {{
            width: 100%;
            padding: 15px;
            border: 2px solid #e1e5e9;
            border-radius: 10px;
            font-size: 1rem;
            transition: all 0.3s ease;
            font-family: inherit;
        }}

        .form-group input:focus,
        .form-group textarea:focus {{
            outline: none;
            border-color: #667eea;
            box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
        }}

        .form-group textarea {{
            resize: vertical;
            min-height: 120px;
        }}

        .submit-btn {{
            width: 100%;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border: none;
            padding: 15px;
            border-radius: 10px;
            font-size: 1.1rem;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s ease;
            text-transform: uppercase;
            letter-spacing: 1px;
        }}

        .submit-btn:hover {{
            transform: translateY(-2px);
            box-shadow: 0 10px 25px rgba(102, 126, 234, 0.3);
        }}

        .submit-btn:active {{
            transform: translateY(0);
        }}

        .submit-btn:disabled {{
            opacity: 0.6;
            cursor: not-allowed;
            transform: none;
        }}

        .status-message {{
            margin-top: 20px;
            padding: 15px;
            border-radius: 10px;
            text-align: center;
            font-weight: 500;
            display: none;
        }}

        .status-message.success {{
            background-color: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }}

        .status-message.error {{
            background-color: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }}

        .loading {{
            display: none;
            align-items: center;
            justify-content: center;
            margin-top: 10px;
        }}

        .spinner {{
            width: 20px;
            height: 20px;
            border: 2px solid #f3f3f3;
            border-top: 2px solid #667eea;
            border-radius: 50%;
            animation: spin 1s linear infinite;
            margin-right: 10px;
        }}

        @keyframes spin {{
            0% {{ transform: rotate(0deg); }}
            100% {{ transform: rotate(360deg); }}
        }}

        .required {{
            color: #e74c3c;
        }}

        .config-info {{
            margin-bottom: 20px;
            padding: 15px;
            background-color: #e3f2fd;
            border-radius: 10px;
            border-left: 4px solid #667eea;
            font-size: 0.9rem;
            color: #0d47a1;
        }}

        @media (max-width: 480px) {{
            .header h1 {{
                font-size: 2rem;
            }}
            .form-container {{
                padding: 30px 20px;
            }}
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Contact Us</h1>
            <p>We'd love to hear from you</p>
        </div>
        
        <div class=""form-container"">
            <div class=""config-info"">
                <strong>📧 Contact Information:</strong><br>
                Messages will be sent to: {recipientEmail}
            </div>

            <form id=""contactForm"">
                <div class=""form-group"">
                    <label for=""name"">Name <span class=""required"">*</span></label>
                    <input type=""text"" id=""name"" name=""name"" required>
                </div>
                
                <div class=""form-group"">
                    <label for=""email"">Email <span class=""required"">*</span></label>
                    <input type=""email"" id=""email"" name=""email"" required>
                </div>
                
                <div class=""form-group"">
                    <label for=""subject"">Subject <span class=""required"">*</span></label>
                    <input type=""text"" id=""subject"" name=""subject"" required>
                </div>
                
                <div class=""form-group"">
                    <label for=""message"">Message <span class=""required"">*</span></label>
                    <textarea id=""message"" name=""message"" placeholder=""Tell us what's on your mind..."" required></textarea>
                </div>
                
                <button type=""submit"" class=""submit-btn"" id=""submitBtn"">
                    Send Message
                </button>
                
                <div class=""loading"" id=""loading"">
                    <div class=""spinner""></div>
                    <span>Sending your message...</span>
                </div>
                
                <div class=""status-message"" id=""statusMessage""></div>
            </form>
        </div>
    </div>

    <script>
        // Get the current base URL for the Azure Function
        const baseUrl = window.location.origin;
        const sendMailUrl = baseUrl + '/api/SendMail';
        
        // Recipient email from server configuration
        const recipientEmail = '{recipientEmail}';
        
        document.getElementById('contactForm').addEventListener('submit', async function(e) {{
            e.preventDefault();
            
            const submitBtn = document.getElementById('submitBtn');
            const loading = document.getElementById('loading');
            const statusMessage = document.getElementById('statusMessage');
            
            // Get form data
            const formData = {{
                name: document.getElementById('name').value.trim(),
                email: document.getElementById('email').value.trim(),
                subject: document.getElementById('subject').value.trim(),
                message: document.getElementById('message').value.trim()
            }};
            
            // Basic validation
            if (!formData.name || !formData.email || !formData.subject || !formData.message) {{
                showMessage('Please fill in all required fields.', 'error');
                return;
            }}
            
            // Show loading state
            submitBtn.disabled = true;
            loading.style.display = 'flex';
            statusMessage.style.display = 'none';
            
            try {{
                // Prepare email data for the SendMail Azure Function
                const emailData = {{
                    to: recipientEmail,
                    subject: 'Contact Form: ' + formData.subject,
                    body: '<div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;"">' +
                          '<h2 style=""color: #667eea;"">New Contact Form Submission</h2>' +
                          '<div style=""background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0;"">' +
                          '<p><strong>Name:</strong> ' + escapeHtml(formData.name) + '</p>' +
                          '<p><strong>Email:</strong> ' + escapeHtml(formData.email) + '</p>' +
                          '<p><strong>Subject:</strong> ' + escapeHtml(formData.subject) + '</p>' +
                          '</div>' +
                          '<div style=""background-color: white; padding: 20px; border: 1px solid #ddd; border-radius: 5px;"">' +
                          '<h3 style=""color: #333; margin-top: 0;"">Message:</h3>' +
                          '<p style=""line-height: 1.6; color: #555;"">' + escapeHtml(formData.message).replace(/\\n/g, '<br>') + '</p>' +
                          '</div>' +
                          '<hr style=""margin: 30px 0; border: none; border-top: 1px solid #eee;"">' +
                          '<p style=""font-size: 12px; color: #666; text-align: center;"">' +
                          'This message was sent via the contact form at ' + new Date().toLocaleString() +
                          '</p></div>'
                }};
                
                console.log('Sending email to:', sendMailUrl);
                console.log('Email data:', emailData);
                
                const response = await fetch(sendMailUrl, {{
                    method: 'POST',
                    headers: {{
                        'Content-Type': 'application/json',
                    }},
                    body: JSON.stringify(emailData)
                }});
                
                console.log('Response status:', response.status);
                
                let result;
                try {{
                    result = await response.json();
                    console.log('Response data:', result);
                }} catch (parseError) {{
                    console.error('Failed to parse response as JSON:', parseError);
                    throw new Error('Invalid response format from server');
                }}
                
                if (response.ok && !result.error) {{
                    showMessage('Thank you! Your message has been sent successfully.', 'success');
                    document.getElementById('contactForm').reset();
                    
                    // Show message ID if available
                    if (result.messageId) {{
                        console.log('Email sent with Message ID:', result.messageId);
                    }}
                }} else {{
                    throw new Error(result.error || result.details || 'Server returned ' + response.status + ': ' + response.statusText);
                }}
                
            }} catch (error) {{
                console.error('Error sending message:', error);
                
                let errorMessage = 'Sorry, there was an error sending your message. Please try again later.';
                
                if (error.name === 'TypeError' && error.message.includes('fetch')) {{
                    errorMessage = 'Unable to connect to the email service. Please check your connection and try again.';
                }} else if (error.message) {{
                    errorMessage = error.message;
                }}
                
                showMessage(errorMessage, 'error');
            }} finally {{
                // Hide loading state
                submitBtn.disabled = false;
                loading.style.display = 'none';
            }}
        }});
        
        function showMessage(message, type) {{
            const statusMessage = document.getElementById('statusMessage');
            statusMessage.textContent = message;
            statusMessage.className = 'status-message ' + type;
            statusMessage.style.display = 'block';
            
            // Auto-hide success messages after 5 seconds
            if (type === 'success') {{
                setTimeout(function() {{
                    statusMessage.style.display = 'none';
                }}, 5000);
            }}
        }}
        
        function escapeHtml(text) {{
            const div = document.createElement('div');
            div.textContent = text;
            return div.innerHTML;
        }}
        
        // Display current configuration info
        console.log('Contact form initialized');
        console.log('SendMail endpoint:', sendMailUrl);
        console.log('Recipient email:', recipientEmail);
    </script>
</body>
</html>";
    }
}