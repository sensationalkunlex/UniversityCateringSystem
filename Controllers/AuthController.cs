using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using UniversityCateringSystem.Models;
using UniversityCateringSystem.Services;
using static PayPal.BaseConstants;

namespace UniversityCateringSystem.Controllers
{
 
    public class AuthController : Controller
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

        [HttpPost]
        public async Task<IActionResult> RequestOtp([FromBody]string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);


            var otp = _otpService.GenerateOtp();
            await _otpService.SaveOtpAsync(user.Email, otp);
            await _emailService.SendOtpEmailAsync(user.Email, otp);
            HttpContext.Session.SetString("Username", email);
            HttpContext.Session.SetString("Role", user.Role.ToString());
            return Json(new { success = true });
        }

        public IActionResult Login( string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
             return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(OtpLoginRequestVM request)
        {

            string checkUsername = HttpContext.Session.GetString("Username");
            string role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(checkUsername)  || string.IsNullOrEmpty(role))
            { 
                return View();
            }
                var user = await _userService.GetUserByEmailAsync(checkUsername);
            if (user == null)
            {
                return View();
            }

            var isValidOtp = await _otpService.ValidateOtpAsync(checkUsername, request.Otp);
            if (!isValidOtp)
            {
                return View();
            }
           
                var userClaims = new List<Claim>()
                {

                    new Claim("UserName",checkUsername),
                  new Claim(ClaimTypes.Role,role)
                   // new Claim(ClaimTypes.Role, user.Role)
                 };
          

            var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
               
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20)
            };

            await HttpContext.SignInAsync(
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Perform login logic (e.g., generate token, set cookie, etc.)
            if(!string.IsNullOrEmpty(request.returnUrl))
            {
                return Redirect(request.returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
           
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
            // ReturnUrl = returnUrl;
        }
    }

    public class EmailRequest
    {
        public string Email { get; set; }
    }
}
