using Microsoft.AspNetCore.Mvc;
using PayPal.Api;
using UniversityCateringSystem.Services;

namespace UniversityCateringSystem.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPayPalService _payPalService;

        public PaymentController(IPayPalService payPalService)
        {
            _payPalService = payPalService;
        }

        public IActionResult PaymentWithPaypal()
        {
            try
            {
                string payerId = Request.Query["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = $"{Request.Scheme}://{Request.Host}/PayPal/PaymentWithPaypal?";
                    var guid = Guid.NewGuid().ToString();
                    var createdPayment = _payPalService.CreatePayment(baseURI + "guid=" + guid);
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    HttpContext.Session.SetString(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Query["guid"];
                    var executedPayment = _payPalService.ExecutePayment(payerId, HttpContext.Session.GetString(guid));
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                return View("FailureView");
            }
            return View("SuccessView");
        }
    }
}

