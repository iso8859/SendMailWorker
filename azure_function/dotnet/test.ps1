#!/usr/bin/env pwsh

# Test script for Azure Function SendMail
# Make sure the function is running locally with 'func start'

$functionUrl = "http://localhost:7071/api/SendMail"

Write-Host "Testing SendMail Azure Function" -ForegroundColor Green
Write-Host "Function URL: $functionUrl" -ForegroundColor Yellow
Write-Host "=" * 50

# Test 1: Valid email request
Write-Host "`nTest 1: Valid email request" -ForegroundColor Cyan

$testEmail = @{
    to = "test@example.com"
    subject = "Test Email from Azure Function"
    body = @"
<h2>Hello from Azure Function!</h2>
<p>This is a test email sent at $(Get-Date)</p>
<p>If you receive this, the function is working correctly!</p>
<ul>
    <li>✅ HTTP POST processing</li>
    <li>✅ JSON parsing</li>
    <li>✅ Email validation</li>
    <li>✅ Template processing</li>
    <li>✅ SMTP sending</li>
</ul>
"@
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri $functionUrl -Method POST -Body $testEmail -ContentType "application/json"
    Write-Host "✅ Success:" -ForegroundColor Green
    Write-Host ($response | ConvertTo-Json -Depth 2) -ForegroundColor White
}
catch {
    Write-Host "❌ Error:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response body: $responseBody" -ForegroundColor Yellow
    }
}

Write-Host "`n" + "=" * 50

# Test 2: Missing required fields
Write-Host "`nTest 2: Missing required fields" -ForegroundColor Cyan

$invalidEmail = @{
    to = "test@example.com"
    # Missing subject and body
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri $functionUrl -Method POST -Body $invalidEmail -ContentType "application/json"
    Write-Host "Response:" -ForegroundColor White
    Write-Host ($response | ConvertTo-Json -Depth 2) -ForegroundColor White
}
catch {
    Write-Host "Expected error (400 Bad Request):" -ForegroundColor Yellow
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        $errorResponse = $responseBody | ConvertFrom-Json
        Write-Host ($errorResponse | ConvertTo-Json -Depth 2) -ForegroundColor White
    }
}

Write-Host "`n" + "=" * 50

# Test 3: Invalid email format
Write-Host "`nTest 3: Invalid email format" -ForegroundColor Cyan

$invalidEmailFormat = @{
    to = "invalid-email-format"
    subject = "Test"
    body = "Test body"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri $functionUrl -Method POST -Body $invalidEmailFormat -ContentType "application/json"
    Write-Host "Response:" -ForegroundColor White
    Write-Host ($response | ConvertTo-Json -Depth 2) -ForegroundColor White
}
catch {
    Write-Host "Expected error (400 Bad Request):" -ForegroundColor Yellow
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        $errorResponse = $responseBody | ConvertFrom-Json
        Write-Host ($errorResponse | ConvertTo-Json -Depth 2) -ForegroundColor White
    }
}

Write-Host "`n" + "=" * 50

# Test 4: CORS preflight (OPTIONS)
Write-Host "`nTest 4: CORS preflight (OPTIONS)" -ForegroundColor Cyan

try {
    $headers = @{}
    $response = Invoke-WebRequest -Uri $functionUrl -Method OPTIONS -Headers $headers
    Write-Host "✅ OPTIONS request successful" -ForegroundColor Green
    Write-Host "Status Code: $($response.StatusCode)" -ForegroundColor White
    Write-Host "CORS Headers:" -ForegroundColor White
    $response.Headers.GetEnumerator() | Where-Object { $_.Key -like "*Access-Control*" } | ForEach-Object {
        Write-Host "  $($_.Key): $($_.Value)" -ForegroundColor Gray
    }
}
catch {
    Write-Host "❌ OPTIONS request failed:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
}

Write-Host "`n" + "=" * 50
Write-Host "`nTest completed!" -ForegroundColor Green
Write-Host "Note: Make sure to configure SMTP settings in local.settings.json for actual email sending." -ForegroundColor Yellow