using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using UniversityCateringSystem.Models;
using UniversityCateringSystem.Services;

namespace UniversityCateringSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductServices _product;
        private const string Cart = "Cart";
        public HomeController(ILogger<HomeController> logger, IProductServices product)
        {
            _logger = logger;
            _product = product;
        }

        public ActionResult LoadHeader()
        {
            return PartialView("~/Views/_partialView/_header.cshtml");
        }
        public async Task<IActionResult> Index()
        {
           var UserName = User.FindFirstValue(ClaimTypes.Role);
            _product.SeedData();
            var products =await _product.GetProducts();
           
            return View(products);
        }
        public async Task<IActionResult> GetCart()
        {
            string cartInSessionString = HttpContext.Session.GetString(Cart);
            var CartItems = _product.GetCartList(cartInSessionString);
            return Json(CartItems);    
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}