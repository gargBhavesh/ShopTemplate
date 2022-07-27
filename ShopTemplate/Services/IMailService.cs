using ShopTemplate.Models;
namespace ShopTemplate.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
        Task SendPasswordResetEmailAsync(ForgotPasswordRequest request);
        Task SendEmailVerificationCodeAsync(EmailVerificationRequest request);
    }
}
