using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SendMailFunction.Models;
using SendMailFunction.Services;
using System.Net;
using System.Text.Json;

namespace SendMailFunction;

public class SendMailHttpFunction
{
    private readonly ILogger<SendMailHttpFunction> _logger;
    private readonly IEmailService _emailService;

    public SendMailHttpFunction(ILogger<SendMailHttpFunction> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    [Function("SendMail")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", "options")] HttpRequestData req)
    {
        _logger.LogInformation("SendMail function triggered");

        // Handle CORS preflight requests
        if (req.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
        {
            var optionsResponse = req.CreateResponse(HttpStatusCode.OK);
            optionsResponse.Headers.Add("Access-Control-Allow-Origin", "*");
            optionsResponse.Headers.Add("Access-Control-Allow-Methods", "POST, OPTIONS");
            optionsResponse.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
            return optionsResponse;
        }

        // Only accept POST requests
        if (!req.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
        {
            var methodResponse = req.CreateResponse(HttpStatusCode.MethodNotAllowed);
            methodResponse.Headers.Add("Access-Control-Allow-Origin", "*");
            await methodResponse.WriteAsJsonAsync(new EmailResponse 
            { 
                Error = "Method not allowed. Only POST requests are accepted." 
            });
            return methodResponse;
        }

        try
        {
            // Parse request body
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            if (string.IsNullOrWhiteSpace(requestBody))
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                badRequestResponse.Headers.Add("Access-Control-Allow-Origin", "*");
                await badRequestResponse.WriteAsJsonAsync(new EmailResponse 
                { 
                    Error = "Request body is empty" 
                });
                return badRequestResponse;
            }

            var emailRequest = JsonSerializer.Deserialize<EmailRequest>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Validate required fields
            if (emailRequest == null || 
                string.IsNullOrWhiteSpace(emailRequest.To) || 
                string.IsNullOrWhiteSpace(emailRequest.Subject) || 
                string.IsNullOrWhiteSpace(emailRequest.Body))
            {
                var validationResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                validationResponse.Headers.Add("Access-Control-Allow-Origin", "*");
                await validationResponse.WriteAsJsonAsync(new EmailResponse 
                { 
                    Error = "Missing required fields: to, subject, body" 
                });
                return validationResponse;
            }

            _logger.LogInformation("Sending email to {To} with subject '{Subject}'", 
                emailRequest.To, emailRequest.Subject);

            // Send email
            var (success, messageId, error) = await _emailService.SendEmailAsync(emailRequest);

            var response = req.CreateResponse(success ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            //response.Headers.Add("Content-Type", "application/json");

            if (success)
            {
                await response.WriteAsJsonAsync(new EmailResponse 
                { 
                    Message = "Email sent successfully",
                    MessageId = messageId
                });
                _logger.LogInformation("Email sent successfully to {To}, MessageId: {MessageId}", 
                    emailRequest.To, messageId);
            }
            else
            {
                await response.WriteAsJsonAsync(new EmailResponse 
                { 
                    Error = "Failed to send email",
                    Details = error
                });
                _logger.LogError("Failed to send email to {To}: {Error}", emailRequest.To, error);
            }

            return response;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Invalid JSON in request body");
            var jsonErrorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            jsonErrorResponse.Headers.Add("Access-Control-Allow-Origin", "*");
            await jsonErrorResponse.WriteAsJsonAsync(new EmailResponse 
            { 
                Error = "Invalid JSON format in request body",
                Details = ex.Message
            });
            return jsonErrorResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing email request");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            errorResponse.Headers.Add("Access-Control-Allow-Origin", "*");
            await errorResponse.WriteAsJsonAsync(new EmailResponse 
            { 
                Error = "Internal server error",
                Details = ex.Message
            });
            return errorResponse;
        }
    }
}