# SendMail Azure Function

A C# Azure Function that sends emails via SMTP with support for basic authentication and OAuth2 (Gmail).

## How It Works

1. **HTTP Trigger**: The function is triggered by HTTP POST requests
2. **Input Validation**: Validates email format and required fields (to, subject, body)
3. **Template Processing**: Uses `template.html` for email formatting with variable substitution
4. **SMTP Sending**: Connects to SMTP server using MailKit library
5. **Authentication**: Supports both basic (username/password) and OAuth2 authentication
6. **Response**: Returns success/failure status with message ID

## API Usage

### Send Email
```
POST /api/SendMail
Content-Type: application/json

{
  "to": "recipient@example.com",
  "subject": "Your Subject",
  "body": "Your message content here"
}
```

### Response
**Success:**
```json
{
  "message": "Email sent successfully",
  "messageId": "<unique-message-id@smtp-server>"
}
```

**Error:**
```json
{
  "error": "Failed to send email",
  "details": "Error details here"
}
```

## Configuration

Set these environment variables/application settings:

### Required for Basic Authentication
- `SMTP_HOST`: SMTP server hostname (e.g., smtp.gmail.com)
- `SMTP_PORT`: SMTP port (default: 587)
- `SMTP_USE_SSL`: Use SSL/TLS (true/false, default: true)
- `SMTP_USERNAME`: SMTP username
- `SMTP_PASSWORD`: SMTP password
- `FROM_EMAIL`: Sender email address
- `FROM_NAME`: Sender display name
- `AUTH_TYPE`: "basic" for username/password

### Additional for Gmail OAuth2
- `AUTH_TYPE`: "oauth2" for OAuth2 authentication
- `OAUTH2_CLIENT_ID`: Google OAuth2 client ID
- `OAUTH2_CLIENT_SECRET`: Google OAuth2 client secret
- `OAUTH2_REFRESH_TOKEN`: OAuth2 refresh token
- `OAUTH2_ACCESS_TOKEN`: OAuth2 access token

## Local Development

1. **Prerequisites:**
   - .NET 8.0 SDK
   - Azure Functions Core Tools v4
   - Visual Studio 2022 or VS Code

2. **Setup:**
   ```bash
   cd azure_function/dotnet
   dotnet restore
   ```

3. **Configure local settings for local debugging:**
   Use environement variables in production.
   Copy `local.settings.json.example` to `local.settings.json`
   Edit `local.settings.json` with your SMTP credentials

4. **Run locally:**
   ```bash
   func start
   ```

5. **Test:**
   ```bash
   curl -X POST http://localhost:7071/api/SendMail \
     -H "Content-Type: application/json" \
     -d '{
       "to": "test@example.com",
       "subject": "Test Email",
       "body": "This is a test message!"
     }'
   ```

## Deployment to Azure

### Method 1: Azure CLI
```bash
# Login to Azure
az login

# Create resource group
az group create --name SendMailRG --location "East US"

# Create storage account
az storage account create --name sendmailstorage123 --resource-group SendMailRG --location "East US" --sku Standard_LRS

# Create function app
az functionapp create --resource-group SendMailRG --consumption-plan-location "East US" \
  --runtime dotnet --functions-version 4 --name SendMailFunction123 \
  --storage-account sendmailstorage123 --os-type Windows

# Deploy function
func azure functionapp publish SendMailFunction123
```

### Method 2: Visual Studio
1. Right-click project → Publish
2. Choose "Azure Functions Consumption Plan"
3. Create new or select existing Function App
4. Configure application settings
5. Publish

### Method 3: VS Code
1. Install Azure Functions extension
2. Sign in to Azure
3. Deploy to Function App
4. Configure application settings in Azure portal

## Configure Application Settings

In Azure Portal:
1. Go to your Function App
2. Settings → Configuration
3. Add the environment variables listed above
4. Save and restart the function app

## Security Considerations

- Use Azure Key Vault for sensitive credentials
- Enable authentication on the Function App if needed
- Configure CORS settings appropriately
- Use managed identity where possible
- Monitor function logs for security events

## Monitoring

- View logs in Azure Portal → Function App → Monitor
- Set up Application Insights for detailed telemetry
- Configure alerts for failures
- Monitor performance metrics

## Troubleshooting

1. **Authentication Failures**: Check SMTP credentials and server settings
2. **Template Not Found**: Ensure `template.html` is deployed with the function
3. **CORS Issues**: Configure CORS in Function App settings
4. **Timeout Issues**: Check SMTP server connectivity and firewall settings

## Cost Estimation

- Azure Functions: ~$0.20 per million executions
- Storage: ~$0.045 per GB/month
- Application Insights: ~$2.30 per GB ingested

Total estimated cost for 10,000 emails/month: ~$1-2

## Example SMTP Configurations

### Gmail
```
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USE_SSL=true
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=your-app-password
AUTH_TYPE=basic
```

### Outlook/Hotmail
```
SMTP_HOST=smtp-mail.outlook.com
SMTP_PORT=587
SMTP_USE_SSL=true
SMTP_USERNAME=your-email@outlook.com
SMTP_PASSWORD=your-password
AUTH_TYPE=basic
```

### Custom SMTP Server
```
SMTP_HOST=mail.yourserver.com
SMTP_PORT=587
SMTP_USE_SSL=true
SMTP_USERNAME=your-username
SMTP_PASSWORD=your-password
AUTH_TYPE=basic
```