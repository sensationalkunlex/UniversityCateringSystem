using UniversityCateringSystem.Models;

namespace UniversityCateringSystem.Services
{
    public interface IProductServices
    {
        //Task ApplicationUpdateAsync();
        List<CartItem> GetCartList(string cartInSessionString);
        Task< List<Product>> GetProducts();
        Task<Product> GetProductsById(int Id);
        void SeedData();
        void SendMail();
    }
}