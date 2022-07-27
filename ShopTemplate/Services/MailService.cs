using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Threading.Tasks;
using ShopTemplate.Models;
using System.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Web;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ShopTemplate.Services
{
    
        public class MailService : IMailService
        {
            private readonly MailSettings _mailSettings;
            private readonly IHttpContextAccessor _httpContextAccessor;
            public MailService(IOptions<MailSettings> mailSettings , IHttpContextAccessor httpContextAccessor)
            {
                _mailSettings = mailSettings.Value;
            _httpContextAccessor = httpContextAccessor;
            }

        
        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            using (var smtp = new SmtpClient()) {
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);

            }  
        }

        public async Task SendPasswordResetEmailAsync(ForgotPasswordRequest request)
        {
            String hostName, scheme;
            
            hostName = _httpContextAccessor.HttpContext.Request.Host.ToString();
            scheme = _httpContextAccessor.HttpContext.Request.Scheme.ToString();

            string FilePath = Directory.GetCurrentDirectory() + "\\Templates\\ResetPasswordMail.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();
            MailText = MailText.Replace("[ResetLink]", request.ResetLink);
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(request.ToEmail));
            email.Subject = "Password Reset Request";
            var builder = new BodyBuilder();
            builder.HtmlBody = MailText;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        public async Task SendEmailVerificationCodeAsync(EmailVerificationRequest request)
        {
            String hostName, scheme;
            
            hostName = _httpContextAccessor.HttpContext.Request.Host.ToString();
            scheme = _httpContextAccessor.HttpContext.Request.Scheme.ToString();

            string FilePath = Directory.GetCurrentDirectory() + "\\Templates\\VerifyEmailAddress.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();
            MailText = MailText.Replace("[Name]", request.Name).Replace("[VerificationCode]", request.VerificationCode);
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(request.ToEmail));
            email.Subject = "Email Verification - Shopivana";
            var builder = new BodyBuilder();
            builder.HtmlBody = MailText;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        
    }
}