using Microsoft.EntityFrameworkCore;

using static System.Formats.Asn1.AsnWriter;

namespace UniversityCateringSystem.Models
{
    public class Invoice:BaseEntity
    {
        public string InvoiceNumber { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentId { get; set; }
        public string? PayerId { get; set; }
        public string Currency { get; set; } = "GBP"; 
        public string? State { get; set; }
        public DateTime CreatedDate { get; set; }= DateTime.UtcNow;
        public List<CartList> CartLists { get; set;}
        public PaymentType? PaymentType { get; set; }
        public TransactionStatus? TransactionStatus { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public string redirectUrl { get;  set; }
        public string InvoiceGuid { get;  set; }
    }
    public class CartList: BaseEntity
    {
        
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public Invoice Invoice { get; set; }
        public Guid InvoiceId { get; set; }
        public string Currency { get; set; } = "GBP";
    }
    public enum PaymentType
    {
        Paypal,
        PayLater,
    }
    public enum TransactionStatus
    {
        Failed,
        Successful,
        Pending,
        Cancel,
    }

}
