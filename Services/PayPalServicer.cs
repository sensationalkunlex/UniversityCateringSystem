using PayPal.Api;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace UniversityCateringSystem.Services
{
    public class PayPalServicer: IPayPalService
    {
        private readonly IConfiguration _configuration;

        public PayPalServicer(IConfiguration configuration)
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



    public class PayPalService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IServiceProvider _serviceProvider;

        public PayPalService(IConfiguration configuration, HttpClient httpClient, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _serviceProvider = serviceProvider;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var clientId = _configuration["PayPal:ClientId"];
            var clientSecret = _configuration["PayPal:ClientSecret"];
            var base64Authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Authorization);

            var requestBody = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await _httpClient.PostAsync($"{_configuration["PayPal:ApiBaseUrl"]}/v1/oauth2/token", requestBody);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(jsonResponse);
            return jsonDocument.RootElement.GetProperty("access_token").GetString();
        }

        public async Task<string> CreatePaymentAsync(string redirectUrl, string invoiceNumber)
        {
            var accessToken = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var payment = new
            {
                intent = "sale",
                payer = new { payment_method = "paypal" },
                transactions = new[]
                {
                new
                {
                    amount = new {
                        total = "44.00", 
                        currency = "GBP", 
                        details = new
                        { 
                            subtotal = "43.00", 
                            tax = "1.00", 
                         

                           
                        }
                    },
                    description = "Transaction description",
                    invoice_number = invoiceNumber,
                    item_list = new { items = new[] { 
                        new { name = "Item Name", 
                            currency = "GBP", 
                            price = "43.00", 
                            quantity = "1", 
                            sku = "sku" } 
                    }
                    }
                }
            },
                redirect_urls = new { return_url = redirectUrl, cancel_url = $"{redirectUrl}&Cancel=true" }
            };

            var content = new StringContent(JsonSerializer.Serialize(payment), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_configuration["PayPal:ApiBaseUrl"]}/v1/payments/payment", content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return jsonResponse;
        }

        public async Task<string> ExecutePaymentAsync(string payerId, string paymentId)
        {
            var accessToken = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var executePayment = new { payer_id = payerId };
            var content = new StringContent(JsonSerializer.Serialize(executePayment), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_configuration["PayPal:ApiBaseUrl"]}/v1/payments/payment/{paymentId}/execute", content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            // Parse the response and save the invoice
            var executedPayment = JsonDocument.Parse(jsonResponse);
            if (executedPayment.RootElement.GetProperty("state").GetString().ToLower() == "approved")
            {
                var transaction = executedPayment.RootElement.GetProperty("transactions")[0];
               
            }

            return jsonResponse;
        }
    }
}
