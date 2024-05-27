﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UniversityCateringSystem.Models;
using UniversityCateringSystem.Services;
using UniversityCateringSystem.Utils;

namespace UniversityCateringSystem.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductServices _productServices;
        public CartController(IProductServices productServices)
        {
            _productServices = productServices;
        }
        private const string Cart = "Cart";
      
        [HttpPost]
        public async Task<IActionResult> AddToCartAsync([FromBody]int productId)
        {
            var product=await _productServices.GetProductsById(productId);
            // Retrieve cart from session or database
            var cart = new List<CartItem>();
            string cartInSessionString = HttpContext.Session.GetString(Cart);
            if (!String.IsNullOrEmpty(cartInSessionString))
            {
                var item = JsonSerializer.Deserialize<List<CartItem>>(cartInSessionString);
                
                cart.AddRange(item);
            }
            var checkCart=cart.FirstOrDefault(x=>x.ProductId==productId);

            // Add new item to cart
            if (checkCart!=null)
            {
                cart.First(x => x.ProductId == productId).Quantity++;
            }
            else
            cart.Add(new CartItem
            {
                ProductId = productId,
                ProductName =product.Name,
                Price = product.Price,
                Quantity = 1
            });

            // Update session
            HttpContext.Session.SetString(Cart, JsonSerializer.Serialize(cart)); 

            return Json(cart);
        }
        [HttpPost]
        public async Task<IActionResult> AddReduceMore( int productId, bool isActionAdd)
        {
            var product = await _productServices.GetProductsById(productId);
            // Retrieve cart from session or database
            var cart = new List<CartItem>();
            if (isActionAdd)
                cart = AddToCart(product);
            else
                cart = RemoveFromCart(product);
            // Update session
            HttpContext.Session.SetString(Cart, JsonSerializer.Serialize(cart));

            return Json(new { success=true});
        }
        private List<CartItem> AddToCart(Product product)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(Cart) ?? new List<CartItem>(); ;
            CartItem? checkCart = FetchCart(product);
            // Add new item to cart
            if (checkCart != null)
            {
                cart.First(x => x.ProductId == product.Id).Quantity++;
            }
            else
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1
                });
            return cart;

        }
        [HttpGet]
        public IActionResult GetCartItems()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var cartTotal = cart.Sum(i => i.Quantity * i.Price);

            var response = new
            {
                success = true,
                cartItems = cart.Select(item => new {
                    id = item.ProductId,
                    productName = item.ProductName,
                    quantity = item.Quantity,
                    priceFormatted = string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-GB"), "{0:C}", item.Price),
                    itemTotalFormatted = string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-GB"), "{0:C}", item.Quantity * item.Price)
                }).ToList(),
                cartTotal = string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-GB"), "{0:C}", cartTotal)
            };

            return Json(response);
        }


        private List<CartItem> RemoveFromCart(Product product)
        {
            List<CartItem> cart= HttpContext.Session.GetObjectFromJson<List<CartItem>>(Cart) ?? new List<CartItem>();
            CartItem checkCart = FetchCart(product);
            // Add new item to cart
            if (checkCart != null)
            {
                if (checkCart.Quantity == 1)
                {
                    cart.RemoveAll(x=>x.ProductId== checkCart.ProductId);
                }
                else
                cart.First(x => x.ProductId == product.Id).Quantity--;

            }


            return cart;

        }

        private CartItem? FetchCart(Product product)
        {
            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(Cart) ?? new List<CartItem>();


            var checkCart = cart.FirstOrDefault(x => x.ProductId == product.Id);
            return checkCart;
        }

        public ActionResult Checkout()
        {
            
            return View();
        }
    }
}
