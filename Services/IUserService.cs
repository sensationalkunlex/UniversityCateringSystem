
using UniversityCateringSystem.Models;

namespace UniversityCateringSystem.Controllers
{
    public interface IUserService
    {
        Task<Invoice?> GetInvoiceByNumber(string InvoiceNumber);
        Task<Invoice?> GetInvoiceByPayerId(string InvoiceNumber);
        Task<User> GetUserByEmailAsync(string email);
        Task InsertInvoice(Invoice invoice);
        Task UpdateUserAsync(string email, string name);
    }
}
