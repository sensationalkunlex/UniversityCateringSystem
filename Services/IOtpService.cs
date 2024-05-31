namespace UniversityCateringSystem.Services
{

    public interface IOtpService

    {
        Task<bool> ValidateOtpAsync(string email, string otp);
        string GenerateOtp();
        Task SaveOtpAsync(string email, string otp);
    }
}
