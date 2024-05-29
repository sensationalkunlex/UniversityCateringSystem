using Microsoft.AspNetCore.Http;
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
                ImageUrl=product.imageUrl,
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
                cart = AddSingleToCart(product);
            else
                cart = RemoveFromCart(product);
            // Update session
            HttpContext.Session.SetString(Cart, JsonSerializer.Serialize(cart));

            return Json(new { success=true});
        }
        private List<CartItem> AddSingleToCart(Product product)
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
                    ImageUrl= product.imageUrl,
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
                    imageUrl= item.ImageUrl,
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
        [HttpPost]
        public async Task<ActionResult> AddToCartUpdateAsync( [FromBody] CartItem cartItem)
        {
        var product = await _productServices.GetProductsById(cartItem.ProductId);
        product.Qty=cartItem.Quantity;
        AddAllToCart(product);
        return Ok(new { message = "Product added to cart successfully!" +JsonSerializer.Serialize(cartItem) });
        }
        private void AddAllToCart(Product product)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(Cart) ?? new List<CartItem>(); ;
            CartItem? checkCart = FetchCart(product);
            // Add new item to cart
            if (checkCart != null)
            {
                cart.First(x => x.ProductId == product.Id).Quantity=product.Qty;
            }
            else
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ImageUrl= product.imageUrl,
                    Price = product.Price,
                    Quantity = product.Qty
                });
            HttpContext.Session.SetString(Cart, JsonSerializer.Serialize(cart));
        }
        
        public async Task<ActionResult> Food(int Id)
        {
            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            var checkCart = await Task.Run(() => cart.FirstOrDefault(x => x.ProductId == Id));

            if (checkCart == null)
            {
                var GetProduct = await _productServices.GetProductsById(Id);
                if (GetProduct != null)
                {
                    checkCart = new CartItem
                    {
                        ProductId = GetProduct.Id,
                        ProductName = GetProduct.Name,
                        Price = GetProduct.Price,
                        Quantity = 0,
                        ImageUrl=GetProduct.imageUrl
                    };
                
                }
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return View(checkCart);

        }
    }
}
