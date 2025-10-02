 Create a Azure Function in C#. This http triggered function receive a POST with an email, a subject and a body and create a mail that it send to predefined SMTP server. This server authenticate with login/password or with AUTH2 like GMail.
 All the configuration (SMTP server, port, login, password, email sender) are in environment variables. Use only SMTP, no other providers.
 The mail body template is in a file named template.html in the root of the project.
 Create very concise instruction file on how it works and how to deploy.