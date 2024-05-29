


namespace UniversityCateringSystem.Controllers
{
        Task<bool> ValidateOtpAsync(object id, string otp);
    internal interface IOtpService
        Task SaveOtpAsync(object id, object otp);
    {
        Task<bool> ValidateOtpAsync(object id, string otp);
        object GenerateOtp();
    }
}
