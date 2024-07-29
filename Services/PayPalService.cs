
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using UniversityCateringSystem.Models;
using UniversityCateringSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace UniversityCateringSystem.Services
{


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
       
        public async Task<string> ExecutePaymentAsync(string payerId, string paymentId)
        {
            var accessToken = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var executePayment = new { payer_id = payerId };
            var content = new StringContent(JsonSerializer.Serialize(executePayment), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_configuration["PayPal:ApiBaseUrl"]}/v1/payments/payment/{paymentId}/execute", content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var executedPayment = JsonDocument.Parse(jsonResponse);
            // Parse the response and save the invoice
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var updateRecord = dbContext.Invoices.FirstOrDefault(x => x.PaymentId == paymentId);
                updateRecord.State = executedPayment.RootElement.GetProperty("state").GetString();
                if (updateRecord.State == "approved")
                updateRecord.TransactionStatus=TransactionStatus.Successful;
                else
                    updateRecord.TransactionStatus = TransactionStatus.Failed;
                updateRecord.PayerId = payerId;

                await dbContext.SaveChangesAsync();
            }

            return jsonResponse;
        }
    
    public async Task<string> CreatePaymentAsync( object payment, Invoice invoice)
        {
            var accessToken = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var content = new StringContent(JsonSerializer.Serialize(payment), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_configuration["PayPal:ApiBaseUrl"]}/v1/payments/payment", content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var executedPayment = JsonDocument.Parse(jsonResponse);
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    invoice.PaymentId = executedPayment.RootElement.GetProperty("id").GetString();
                    dbContext.Invoices.Add(invoice);
                    await dbContext.SaveChangesAsync();
                }
            }catch(Exception ex)
            {

            }
         
            return jsonResponse;
        }

        }
}
