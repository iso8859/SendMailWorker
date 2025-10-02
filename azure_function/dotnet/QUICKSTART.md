# Quick Start Guide - SendMail Azure Function

## 🚀 Get Started in 5 Minutes

### 1. Prerequisites
- .NET 8.0 SDK
- Azure Functions Core Tools v4

### 2. Quick Setup
```bash
cd azure_function/dotnet
dotnet restore
```

### 3. Configure SMTP
Edit `local.settings.json`:
```json
{
  "Values": {
    "SMTP_HOST": "smtp.gmail.com",
    "SMTP_PORT": "587",
    "SMTP_USERNAME": "your-email@gmail.com",
    "SMTP_PASSWORD": "your-app-password",
    "FROM_EMAIL": "your-email@gmail.com",
    "FROM_NAME": "Your Name",
    "AUTH_TYPE": "basic"
  }
}
```

### 4. Run Locally
```bash
func start
```

### 5. Test
```bash
# PowerShell
./test.ps1

# Or curl
curl -X POST http://localhost:7071/api/SendMail \
  -H "Content-Type: application/json" \
  -d '{
    "to": "test@example.com",
    "subject": "Test Email",
    "body": "Hello from Azure Function!"
  }'
```

## 📧 SMTP Provider Quick Configs

### Gmail
1. Enable 2FA on your Gmail account
2. Generate an App Password
3. Use these settings:
```
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=your-16-digit-app-password
```

### Outlook/Hotmail
```
SMTP_HOST=smtp-mail.outlook.com
SMTP_PORT=587
SMTP_USERNAME=your-email@outlook.com
SMTP_PASSWORD=your-password
```

### Other Providers
- **Yahoo**: smtp.mail.yahoo.com:587
- **Custom**: Check your email provider's SMTP settings

## 🌐 Deploy to Azure

### Option 1: Azure CLI (Fastest)
```bash
# Login and create resources
az login
az group create --name SendMailRG --location "East US"
az functionapp create --resource-group SendMailRG --consumption-plan-location "East US" \
  --runtime dotnet --functions-version 4 --name SendMailFunction$(Get-Random) \
  --storage-account sendmailstorage$(Get-Random)

# Deploy
func azure functionapp publish YourFunctionAppName
```

### Option 2: Visual Studio
1. Right-click project → Publish
2. Choose Azure Functions
3. Create/select Function App
4. Publish

## ⚙️ Configure in Azure
1. Go to Azure Portal → Your Function App
2. Settings → Configuration
3. Add your SMTP settings as Application Settings
4. Save and restart

## 🔒 Security Tips
- Use Azure Key Vault for passwords
- Enable Function App authentication if needed
- Set up proper CORS settings
- Monitor with Application Insights

## 📊 Monitoring
- View logs: Azure Portal → Function App → Monitor
- Real-time logs: `func azure functionapp logstream YourFunctionAppName`
- Set up alerts for failures

## 💰 Cost Estimate
- **Free Tier**: 1M executions/month free
- **After Free Tier**: ~$0.20 per million executions
- **Storage**: ~$1-2/month
- **Total for 10K emails/month**: Essentially free

## 🛠️ Troubleshooting

| Issue | Solution |
|-------|----------|
| Authentication failed | Check SMTP credentials, enable "Less secure apps" for Gmail, or use App Password |
| Template not found | Ensure `template.html` is in project root and set to "Copy to Output Directory" |
| CORS errors | Add CORS settings in Function App configuration |
| Timeout | Check firewall settings, try different SMTP port (25, 465, 587) |

## 📞 Support
- Check Azure Function logs for detailed error messages
- Test SMTP settings with a desktop email client first
- Use Application Insights for detailed telemetry

---
**Ready to send emails!** 🎉