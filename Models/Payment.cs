namespace UniversityCateringSystem.Models
{
    public class Payment
    {
        public string Intent { get; set; }
        public Payer Payer { get; set; }
        public List<Transaction> Transactions { get; set; }
    }

    public class Payer
    {
        public string PaymentMethod { get; set; }
    }

    public class Transaction
    {
        public Amount Amount { get; set; }
        public string Description { get; set; }
        public string InvoiceNumber { get; set; }
        public ItemList ItemList { get; set; }
    }

    public class Amount
    {
        public string Total { get; set; }
        public string Currency { get; set; }
        public Details Details { get; set; }
    }

    public class Details
    {
        public string Subtotal { get; set; }
        public string Tax { get; set; }
    }

    public class ItemList
    {
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        public string Name { get; set; }
        public string Currency { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
        public string Sku { get; set; }
    }

}
