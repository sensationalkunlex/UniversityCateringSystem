using UniversityCateringSystem.Models;

namespace UniversityCateringSystem.Services
{
	public interface IProductServices
	{
		//Task ApplicationUpdateAsync();
		List<CartItem> GetCartList(string cartInSessionString);
		Task< List<Product>> GetProducts();
		Task<List<Product>> GetProductsBySearchable(string query);
		Task<Product> GetProductsById(int Id);
		Task<List<Product>> GetProductLists(string query, int page, int pageSize);
	  
	}
}