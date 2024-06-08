using Microsoft.EntityFrameworkCore;
using UniversityCateringSystem.Data;
using UniversityCateringSystem.Models;

namespace UniversityCateringSystem.Utils
{
   public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (
                var context = new AppDbContext(
                    serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()
                )
            )
            {
                if (!context.Products.Any())
                {
                    //Generate food product list.
                    var foodProducts = GenerateFoodProducts();

                    // Add products to the context and save changes
                    foreach (var product in foodProducts)
                    {
                        var newProduct = new Product
                        {
                            Name = product.Item1,
                            Price = GenerateRandomPrice(),
                            Description = GenerateDescription(product.Item1),
                            Qty = GenerateRandomQuantity(),
                            imageUrl = product.Item2,
                        };
                        context.Products.Add(newProduct);
                    }
                    context.SaveChanges();
                }
            }
        }

        static List<(string, string)> GenerateFoodProducts()
        {
            var baseProducts = new List<(string Name, string ImageUrl)>
            {
                ("Fish and Chips", "/img/foods/Fish_and_chips_blackpool.jpg"),
                ("Full English Breakfast", "/img/foods/full-english.jpg"),
                ("Jacket Potato", "/img/foods/air-fryer-baked-potatoes.jpg"),
                ("Burger and Fries", "/img/foods/burger+and+fries.jpg"),
                ("Pizza", "/img/foods/classic-cheese-pizza.jpg"),
                ("Sandwich", "/img/foods/cucumber-caprese-sandwich.jpg"),
                ("Soup", "/img/foods/chickensoup.jpg"),
                ("Salad", "/img/foods/green-salad.jpg"),
                ("Pasta", "/img/foods/pasta.jpg"),
                ("Curry", "/img/foods/curry.jpg"),
                ("Stir Fry", "/img/foods/pork-noodle-stir-fry.jpg"),
                ("Sushi", "/img/foods/Sushi.jpeg"),
                ("Bakery Items", "/img/foods/baked.jpg"),
                ("Desserts", "/img/foods/Desserts.jpg"),
                ("Beverages", "/img/foods/Beverages.jpg"),
                ("Smoothies", "/img/foods/Smoothies.jpg"),
                ("Ice Cream", "/img/foods/IceCream.jpg"),
            };

            var extendedProducts = new List<(string, string)>();

            for (int i = 0; i < 1000; i++)
            {
                var baseProduct = baseProducts[i % baseProducts.Count];
                var newProductName = $"{baseProduct.Name} {i + 1}";
                extendedProducts.Add((newProductName, baseProduct.ImageUrl));
            }

            return extendedProducts;
        }

        static string GenerateDescription(string productName)
        {
            var descriptions = new List<string>
            {
                "Delicious and tasty, perfect for any occasion.",
                "A classic favorite that's sure to please.",
                "Made with the finest ingredients for superior taste.",
                "A delightful treat for your taste buds.",
                "Perfect for a quick meal or snack.",
                "A nutritious and satisfying choice.",
                "Indulge in this scrumptious offering.",
                "A beloved dish that's always in style.",
                "Prepared to perfection for your enjoyment.",
                "A mouthwatering selection you'll love."
            };

            var random = new Random();
            return descriptions[random.Next(descriptions.Count)];
        }

        static decimal GenerateRandomPrice()
        {
            var random = new Random();
            return Math.Round((decimal)random.NextDouble() * 100, 2);
        }

        static int GenerateRandomQuantity()
        {
            var random = new Random();
            return random.Next(1, 100);
        }
    }
}
