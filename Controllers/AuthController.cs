using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UniversityCateringSystem.Models;
using UniversityCateringSystem.Services;

namespace UniversityCateringSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly IOtpService _otpService;

        public AuthController(IEmailService emailService, IUserService userService, IOtpService otpService)
        {
            _emailService = emailService;
            _userService = userService;
            _otpService = otpService;
        }

        [HttpPost("request-otp")]
        public async Task<IActionResult> RequestOtp([FromBody] EmailRequest request)
        {
            var user = await _userService.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return NotFound("Email not found.");
            }

            var otp = _otpService.GenerateOtp();
            await _otpService.SaveOtpAsync(user.Id, otp);
            await _emailService.SendOtpEmailAsync(user.Email, otp);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] OtpLoginRequest request)
        {
            var user = await _userService.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return NotFound("Email not found.");
            }

            var isValidOtp = await _otpService.ValidateOtpAsync(user.Id, request.Otp);
            if (!isValidOtp)
            {
                return Unauthorized("Invalid OTP.");
            }

            // Perform login logic (e.g., generate token, set cookie, etc.)

            return Ok(new { message = "Login successful." });
        }
    }

    public class EmailRequest
    {
        public string Email { get; set; }
    }
}
