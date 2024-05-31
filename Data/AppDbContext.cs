using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using UniversityCateringSystem.Models;

namespace UniversityCateringSystem.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OtpLoginRequest> OtpLoginRequests { get; set; }
        public DbSet<Invoice> Invoices {get; set;}
        public DbSet<CartList> CartLists {get; set;}
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
