namespace ShopTemplate.Models
{
    public class ForgotPasswordRequest
    {
        public string ToEmail { get; set; }
        public string ResetLink { get; set; }
    }
}
