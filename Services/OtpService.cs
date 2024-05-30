



using Microsoft.EntityFrameworkCore;
using UniversityCateringSystem.Data;
using UniversityCateringSystem.Models;

namespace UniversityCateringSystem.Controllers
{
    public class OtpService : IOtpService
    {
        private readonly AppDbContext _context;

        public OtpService(AppDbContext context)
        {
            _context = context;
        }

        public string GenerateOtp()
        {

            var random = new Random();
            return new string(Enumerable.Repeat("0123456789", 4)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task SaveOtpAsync(string email, string otp)
        {
            var saveOtp=new OtpLoginRequest { Email = email,
            Otp = otp, IsValid=true };
            var allOtpRequests = await _context.OtpLoginRequests
             .Where(x => x.Email == email && x.IsValid==true)
             .ToListAsync();

            // Mark all existing OTP requests as invalid
            foreach (var otpRequest in allOtpRequests)
            {
                otpRequest.IsValid = false;
            }

            _context.OtpLoginRequests.Add(saveOtp);
            await _context.SaveChangesAsync();
            
        }

        public async Task<bool> ValidateOtpAsync(string email, string otp)
        {
            var twoHoursAgo = DateTime.UtcNow.AddHours(-2);
          return await _context.OtpLoginRequests.FirstOrDefaultAsync(x => x.Email == email && x.CreatedAt >= twoHoursAgo && x.IsValid==true)!=null;

        }
    }
}
