# Contact Page Azure Function

## Overview
A new Azure Function (`ContactPageHttpFunction`) has been added to serve an HTML contact form that integrates with your existing `SendMailHttpFunction`.

## New Features Added

### 1. ContactPageHttpFunction.cs
- **HTTP GET endpoint**: Serves the contact form at `/api/ContactPage`
- **Anonymous access**: No authentication required for viewing the contact form
- **Dynamic configuration**: Uses environment variables for recipient email
- **Integrated design**: Uses the same styling as your standalone contact form

### 2. ContactFormSettings.cs
- Configuration helper for contact form settings
- Automatically uses `CONTACT_RECIPIENT_EMAIL` or falls back to `FROM_EMAIL`

### 3. Updated SendMailHttpFunction.cs
- **Changed authorization level**: From `Function` to `Anonymous` for easier integration
- **CORS support**: Already configured for cross-origin requests

## Configuration

### Environment Variables
Add this to your `local.settings.json` or Azure Function App Settings:

```json
{
  "CONTACT_RECIPIENT_EMAIL": "contact@yourcompany.com"
}
```

If not set, it will use `FROM_EMAIL` as fallback.

## Usage

### Local Development
1. Start your Azure Function:
   ```bash
   cd azure_function/dotnet
   func start
   ```

2. Open the contact form in your browser:
   ```
   http://localhost:7071/api/ContactPage
   ```

### Production Deployment
After deploying to Azure, the contact form will be available at:
```
https://your-function-app.azurewebsites.net/api/ContactPage
```

## API Endpoints

| Endpoint | Method | Purpose | Auth Level |
|----------|--------|---------|------------|
| `/api/ContactPage` | GET | Serves the HTML contact form | Anonymous |
| `/api/SendMail` | POST | Sends emails from the contact form | Anonymous |

## Features

### Contact Form Features
- **Responsive design**: Works on desktop and mobile
- **Real-time validation**: Client-side validation for required fields
- **Loading states**: Visual feedback during email sending
- **Error handling**: Comprehensive error handling with user-friendly messages
- **Email formatting**: Automatically formats contact submissions into professional emails

### Integration Features
- **Self-contained**: All functionality in a single Azure Function app
- **Dynamic recipient**: Email recipient configured via environment variables
- **Professional email format**: Contact submissions are formatted as structured HTML emails
- **Timestamp tracking**: Emails include submission timestamp

## Email Format
Contact form submissions generate emails with:
- Sender's name and email address
- Subject prefixed with "Contact Form:"
- Structured HTML layout with contact details
- Message body with proper formatting
- Timestamp and source attribution

## Security Considerations

### Authentication
- Both functions use `Anonymous` authorization for ease of use
- Consider implementing rate limiting for production use
- Monitor for abuse and implement appropriate safeguards

### Input Validation
- Client-side validation for user experience
- Server-side validation in the SendMail function
- HTML escaping to prevent XSS attacks

## Customization

### Styling
Modify the CSS in the `GetContactPageHtml` method to match your brand

### Email Template
Customize the email body format in the JavaScript section of the contact form

### Form Fields
Add or remove form fields by:
1. Updating the HTML form structure
2. Modifying the JavaScript form data collection
3. Updating the email body template

## Testing

### Test Checklist
- [ ] Contact form loads correctly
- [ ] Form validation works (empty fields, invalid email)
- [ ] Email sending works end-to-end
- [ ] Success/error messages display properly
- [ ] Email recipient receives formatted emails
- [ ] Mobile responsiveness works

### Common Issues
1. **Form doesn't load**: Check if Azure Function is running
2. **Email not sending**: Verify SMTP configuration in environment variables
3. **CORS errors**: Ensure CORS headers are properly set (already configured)
4. **Recipient email wrong**: Check `CONTACT_RECIPIENT_EMAIL` environment variable

## Deployment Notes

### Azure Function App Settings
When deploying to Azure, ensure these environment variables are set:
- `CONTACT_RECIPIENT_EMAIL`: Where contact form emails should be sent
- All existing SMTP settings for email sending

### Custom Domain (Optional)
If using a custom domain, update any hardcoded URLs in the contact form JavaScript.

This implementation provides a complete, self-contained contact solution within your existing Azure Function infrastructure!