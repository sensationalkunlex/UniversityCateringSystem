using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
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
            var products = await _product.GetProducts();
            return View(products);
        }

        public async Task<IActionResult> GetProducts(
            string query = "",
            int page = 1,
            int pageSize = 10
        )
        {
            var products = await _product.GetProductLists(query, page, pageSize);
            return Json(new { result = products });
        }

        public async Task<IActionResult> GetSearchable(string query)
        {
            var filteredProducts = await _product.GetProductsBySearchable(query);
            return Json(new { result = filteredProducts });
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
            return View(
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                }
            );
        }
    }
}
