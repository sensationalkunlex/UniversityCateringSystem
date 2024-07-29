using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UniversityCateringSystem.Models;
using UniversityCateringSystem.Services;


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
        public async Task<IActionResult> RequestOtp([FromBody] string email)
        {
            if (!IsValidEmail(email)) {

                return Json(new { success = false,
                    message = "Invalid email address" });
            }

            var user = await _userService.GetUserByEmailAsync(email);

            HttpContext.Session.SetString("Username", email);
            HttpContext.Session.SetString("Role", user.Role.ToString());
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            if (!user.NewUser)
            {
                var otp = _otpService.GenerateOtp();
                await _otpService.SaveOtpAsync(user.Email, otp);
                await _emailService.SendOtpEmailAsync(user.Email, otp);
                return Json(new { success = true, newUser = false });
            }
            return Json(new { success = true, newUser = true });


        }
        [HttpPost]
        public async Task<IActionResult> UpdateUserInfo([FromBody] string name)
        {

            if (string.IsNullOrEmpty(name))
                return Json(new
                {
                    success = false,
                    message = "No name"
                });
            try
            {
                string email = HttpContext.Session.GetString("Username");
                var otp = _otpService.GenerateOtp();
                await _otpService.SaveOtpAsync(email, otp);
                await _emailService.SendOtpEmailAsync(email, otp);
                await _userService.UpdateUserAsync(email, name);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred"
                });
            }


        }


        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Check if email matches a valid email pattern
                var regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
                return regex.IsMatch(email);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public IActionResult Login(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            ViewBag.start = "True";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(OtpLoginRequestVM request)
        {

            string checkUsername = HttpContext.Session.GetString("Username");
            string role = HttpContext.Session.GetString("Role");
            string UserId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(checkUsername) || string.IsNullOrEmpty(role) || string.IsNullOrEmpty(UserId))
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
                ViewBag.start = "False";
                return View();
            }
           
                var userClaims = new List<Claim>()
                {

                   new Claim("UserName",checkUsername),
                  new Claim(ClaimTypes.Role,role),
                  new Claim(ClaimTypes.NameIdentifier,UserId)
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
