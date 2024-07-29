namespace UniversityCateringSystem.Services
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string email, string otp);
    }
}
