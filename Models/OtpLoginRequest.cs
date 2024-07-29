namespace UniversityCateringSystem.Models
{
    public class OtpLoginRequest: BaseEntity
    {
        public string Email { get; set; }
        public string Otp { get; set; }
        public DateTime CreatedAt { get; set; }  = DateTime.UtcNow;
        public bool? IsValid { get; set; }
    }

    public class OtpLoginRequestVM
    {
        public string Email { get; set; }
        
        public string Otp { get; set; }
        public string returnUrl { get; set; }
    }
}
