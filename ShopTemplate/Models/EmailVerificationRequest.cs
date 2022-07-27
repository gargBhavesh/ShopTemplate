namespace ShopTemplate.Models
{
    public class EmailVerificationRequest
    {
        public string Name { get; set; }
        public string VerificationCode { get; set; }

        public string ToEmail { get; set; }
    }
}
