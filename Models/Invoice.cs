using Microsoft.EntityFrameworkCore;
using PayPal.Api;
using static System.Formats.Asn1.AsnWriter;

namespace UniversityCateringSystem.Models
{
    public class Invoice
    {
        public string InvoiceNumber { get; set; }
        public decimal Amount { get; set; }
        public Guid CartId { get; set; }
        public string PaymentId { get; set; }
        public string PayerId { get; set; }
        public string Currency { get; set; }
        public string State { get; set; }
        public DateTime CreatedDate { get; set; }= DateTime.UtcNow;




    }
 

//                using (var scope = _serviceProvider.CreateScope())
//                {
//                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    dbContext.Invoices.Add(invoice);
//                    await dbContext.SaveChangesAsync();
//}
}
