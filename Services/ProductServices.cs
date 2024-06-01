using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Text.Json;
using UniversityCateringSystem.Data;
using UniversityCateringSystem.Models;
using Microsoft.AspNetCore.Hosting;

namespace UniversityCateringSystem.Services
{
    public class ProductServices : IProductServices
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductServices(AppDbContext context, 
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this._webHostEnvironment = webHostEnvironment;
        }
        public async Task<List<Product>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }
        public async Task<Product> GetProductsById(int Id)
        {
            return await _context.Products.SingleAsync(x => x.Id == Id);
        }
        public void SeedData()
        {
            // Clear existing data
            if (_context.Products.ToList().Count == 0)
            {


                //Generate food product list.
                var foodProducts = GenerateFoodProducts();

                // Add products to the context and save changes
                foreach (var productName in foodProducts)
                {
                    var product = new Product
                    {
                        Name = productName.Item1,
                        Price = GenerateRandomPrice(),
                        Description = "Description for " + productName.Item1,
                        Qty = GenerateRandomQuantity(),
                        imageUrl = productName.Item2,
                    };
                    _context.Products.Add(product);
                }
                _context.SaveChanges();

            }
        }

        public List<CartItem> GetCartList(string cartInSessionString)
        {
            var cart = new List<CartItem>();

            if (!String.IsNullOrEmpty(cartInSessionString))
            {
                var item = JsonSerializer.Deserialize<List<CartItem>>(cartInSessionString);

                cart.AddRange(item);
            }
            return cart;
        }


        List<(string, string)> GenerateFoodProducts()
        {

            var foodProducts = new List<(string Name, string ImageUrl)>
            {
    ("Fish and Chips", "https://upload.wikimedia.org/wikipedia/commons/f/ff/Fish_and_chips_blackpool.jpg"),
    ("Full English Breakfast", "https://iamafoodblog.b-cdn.net/wp-content/uploads/2019/02/full-english-7355w-2-1024x683.jpg"),
    ("Jacket Potato", "https://images.immediate.co.uk/production/volatile/sites/30/2022/07/air-fryer-baked-potatoes44-d197a3e.jpg?quality=90&resize=556,505"),
    ("Burger and Fries", "https://images.squarespace-cdn.com/content/v1/61c38dbdda885109e22a3868/1663951873993-TYZBQUIBC23O25OE9RDK/burger+and+fries.jpg"),
    ("Pizza", "https://www.foodandwine.com/thmb/Wd4lBRZz3X_8qBr69UOu2m7I2iw=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/classic-cheese-pizza-FT-RECIPE0422-31a2c938fc2546c9a07b7011658cfd05.jpg"),
    ("Sandwich", "https://www.eatingwell.com/thmb/LQXwKPvgYghCs2LH7bwlsx0gD1Q=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/cucumber-caprese-sandwich-6343482dece14c3d876bc7bac317ecd8.jpg"),
    ("Soup", "https://ichef.bbci.co.uk/food/ic/food_16x9_1600/recipes/chickensoup_1918_16x9.jpg"),
    ("Salad", "https://cdn.loveandlemons.com/wp-content/uploads/2021/04/green-salad.jpg"),
    ("Pasta", "https://assets.epicurious.com/photos/5988e3458e3ab375fe3c0caf/1:1/w_3607,h_3607,c_limit/How-to-Make-Chicken-Alfredo-Pasta-hero-02082017.jpg"),
    ("Curry", "https://www.allrecipes.com/thmb/FL-xnyAllLyHcKdkjUZkotVlHR8=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/46822-indian-chicken-curry-ii-DDMFS-4x3-39160aaa95674ee395b9d4609e3b0988.jpg"),
    ("Stir Fry", "https://images.immediate.co.uk/production/volatile/sites/30/2020/08/pork-noodle-stir-fry-3cb19c3.jpg"),
    ("Sushi", "https://www.justonecookbook.com/wp-content/uploads/2020/01/Sushi-Rolls-Maki-Sushi-%E2%80%93-Hosomaki-1106-II.jpg"),
    ("Bakery Items", "https://cdn.cheapism.com/images/Croissant.2e16d0ba.fill-1440x605.png"),
    ("Desserts", "https://images.immediate.co.uk/production/volatile/sites/30/2020/08/dessert-main-image-molten-cake-0fbd4f2.jpg?quality=90&resize=500,454"),
    ("Beverages", "https://s7d1.scene7.com/is/image/KeminIndustries/shutterstock_519547867?$responsive$"),
    ("Smoothies", "https://hips.hearstapps.com/hmg-prod/images/delish-how-to-make-a-smoothie-horizontal-1542310071.png?crop=0.8893333333333334xw:1xh"),
    ("Ice Cream", "https://www.allrecipes.com/thmb/SI6dn__pfJb9G5eBpYAqkyGCLxQ=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/50050-five-minute-ice-cream-DDMFS-4x3-076-fbf49ca6248e4dceb3f43a4f02823dd9.jpg"),

};




            return foodProducts;
        }

        decimal GenerateRandomPrice()
        {
            var random = new Random();
            return Math.Round((decimal)random.NextDouble() * 1, 2);
        }

        int GenerateRandomQuantity()
        {
            var random = new Random();
            return random.Next(1, 100);
        }

        
      }
}
