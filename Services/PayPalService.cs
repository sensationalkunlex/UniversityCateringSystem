using PayPal.Api;

namespace UniversityCateringSystem.Services
{
    public class PayPalService: IPayPalService
    {
        private readonly IConfiguration _configuration;

        public PayPalService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private APIContext GetAPIContext()
        {
            var clientId = _configuration["PayPal:ClientId"];
            var clientSecret = _configuration["PayPal:ClientSecret"];
            var config = new Dictionary<string, string>
        {
            { "mode", _configuration["PayPal:Mode"] }
        };
            var accessToken = new OAuthTokenCredential(clientId, clientSecret, config).GetAccessToken();
            return new APIContext(accessToken);
        }

        public Payment CreatePayment(string redirectUrl)
        {
            var clientId = _configuration["PayPal:ClientId"];
            var clientSecret = _configuration["PayPal:ClientSecret"];

            var client = new PaypalClient(clientId, clientSecret, "Test");
            var checker = client.Authenticate().Result;
            var apiContext = GetAPIContext();

            var itemList = new ItemList()
            {
                items = new List<Item>()
            {
                new Item()
                {
                    name = "Item Name",
                    currency = "USD",
                    price = "10",
                    quantity = "1",
                    sku = "sku"
                }
            }
            };

            var payer = new Payer() { payment_method = "paypal" };

            var redirectUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            var details = new Details()
            {
                tax = "1",
                shipping = "1",
                subtotal = "10"
            };

            var amount = new Amount()
            {
                currency = "USD",
                total = "12",
                details = details
            };

            var transactionList = new List<Transaction>();
            transactionList.Add(new Transaction()
            {
                description = "Transaction description",
                invoice_number = "your_invoice_number",
                amount = amount,
                item_list = itemList
            });

            var payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirectUrls
            };

            return payment.Create(apiContext);
        }

        public Payment ExecutePayment(string payerId, string paymentId)
        {
            var apiContext = GetAPIContext();
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            var payment = new Payment() { id = paymentId };
            return payment.Execute(apiContext, paymentExecution);
        }
    }

}
