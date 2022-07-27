using Microsoft.AspNetCore.Mvc;
using ShopTemplate.Services;
using ShopTemplate.Models;

namespace ShopTemplate.Controllers
{
    
    public class MailController : ControllerBase
    {
        private readonly IMailService mailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MailController(IMailService mailService, IHttpContextAccessor httpContextAccessor)
        {
            this.mailService = mailService;
            this._httpContextAccessor = httpContextAccessor;
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendMail([FromForm] MailRequest request)
        {
            try
            {
                await mailService.SendEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        
        
        public async Task<IActionResult> SendPasswordResetEmail( ForgotPasswordRequest request)
        {
            try
            {
                await mailService.SendPasswordResetEmailAsync(request);
                
                return RedirectToAction("ForgotPasswordConfirmation","User");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> SendEmailVerificationCode( EmailVerificationRequest request)
        {
            bool returnVal = false;
            try
            {
                await mailService.SendEmailVerificationCodeAsync(request);

                returnVal=  true;
            }
            catch (Exception ex)
            {
                returnVal = false; 
            }
            return returnVal;
        }
        [HttpPost]
        public async Task<bool> VerifyEmail()
        {
            bool returnVal = true;
            try
            {
                string email = HttpContext.Session.GetString("EmailId");
                string verificationCode = RandomString(8);
                string name = HttpContext.Session.GetString("UserName");
                EmailVerificationRequest request = new EmailVerificationRequest();
                request.Name = name;
                request.VerificationCode = verificationCode;
                request.ToEmail = email;

                bool emailVerifyTask = await  SendEmailVerificationCode(request);

                
                returnVal = emailVerifyTask;
                HttpContext.Session.SetString("VerificationCode", verificationCode);

                //bool retVal =  RedirectToAction("SendEmailVerificationCode", "Mail", request);
            }
            catch
            {
                returnVal = false;
            }
            return returnVal;

        }
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
