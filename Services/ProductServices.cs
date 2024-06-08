using System.Net;
using System.Net.Mail;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityCateringSystem.Data;
using UniversityCateringSystem.Models;

namespace UniversityCateringSystem.Services
{
	public class ProductServices : IProductServices
	{
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public ProductServices(AppDbContext context, IWebHostEnvironment webHostEnvironment)
		{
			_context = context;
			this._webHostEnvironment = webHostEnvironment;
		}

		public async Task<List<Product>> GetProducts()
		{
			return await _context.Products.ToListAsync();
		}

		public async Task<List<Product>> GetProductsBySearchable(string query)
		{
			return await _context
				.Products.Where(x =>
					x.Name.ToLower().StartsWith(query.ToLower())
					|| x.Description.ToLower().StartsWith(query.ToLower())
				)
				.ToListAsync();
		}

		public async Task<List<Product>> GetProductLists(string query, int page, int pageSize)
		{
			var productQuery = _context.Products;

			if (!string.IsNullOrEmpty(query)){
				return await productQuery.Where(x =>
					x.Name.ToLower().Contains(query.ToLower())
					|| x.Description.ToLower().Contains(query.ToLower())
				).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
				}
			return await productQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
		}

		public async Task<Product> GetProductsById(int Id)
		{
			return await _context.Products.SingleAsync(x => x.Id == Id);
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
	}
}
