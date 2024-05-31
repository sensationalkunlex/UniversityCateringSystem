
using Microsoft.AspNetCore.Hosting;
using System.Net.Mail;
using System.Net;

namespace UniversityCateringSystem.Services
{
    public class EmailService : IEmailService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmailService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task SendOtpEmailAsync(string email, string otp)
        {
            try
            {
                MailMessage mailMessage;
                string subject = "OTP";
                ConfigureEmail(subject,email, out mailMessage);
                // mailMessage.Body = "<h1>This is a test email sent from a .NET application using Outlook.</h1>";
                mailMessage.IsBodyHtml = true;
                string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Template/Mail", "otp.html");

                string htmlContent = await File.ReadAllTextAsync(filePath);
                var message = htmlContent.Replace("{{OTP_CODE}}", otp);
                mailMessage.Body = message;
                sendMail(mailMessage);

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while sending the email: " + ex.Message);
            }

        }
        
        private  void ConfigureEmail(string subject, string toEmail,  out MailMessage mailMessage)
        {
          
            mailMessage = new MailMessage();
            mailMessage.To.Add(toEmail);
            mailMessage.Subject = subject;
        }

        private static void sendMail( MailMessage mailMessage)
        {
            string fromEmail = "universitycateringservice@outlook.com";
            string emailPassword = "Uni@2024";
            SmtpClient smtpClient = new SmtpClient("smtp-mail.outlook.com", 587);
            smtpClient.Credentials = new NetworkCredential(fromEmail, emailPassword);
            smtpClient.EnableSsl = true;
            mailMessage.From = new MailAddress(fromEmail);
            smtpClient.Send(mailMessage);
        }
    }
}
