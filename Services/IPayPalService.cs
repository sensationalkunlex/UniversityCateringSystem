using PayPal.Api;

namespace UniversityCateringSystem.Services
{
    public interface IPayPalService
    {
        Payment CreatePayment(string redirectUrl);
        Payment ExecutePayment(string payerId, string paymentId);
    }
}