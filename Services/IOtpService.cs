


namespace UniversityCateringSystem.Controllers
{
        
    public interface IOtpService
        
    {
        Task<bool> ValidateOtpAsync(Guid id, string otp);
        string GenerateOtp();
        Task SaveOtpAsync(Guid id, string otp);
    }
}
