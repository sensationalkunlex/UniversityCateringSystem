using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayPal.Api;
using System.Text.Json;
using UniversityCateringSystem.Services;

namespace UniversityCateringSystem.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPayPalService _payPalServiceold;

        public readonly PayPalService _payPalService;

        public PaymentController(IPayPalService payPalServiceold, PayPalService payPalService)
        {
            _payPalServiceold = payPalServiceold;
            _payPalService = payPalService;
        }
        [Authorize]
        public IActionResult Index()
        {
            ViewBag.PaymentId = Guid.NewGuid().ToString();

            
            return View();
        }
        public IActionResult PaymentWithPaypalOld()
        {
            try
            {
                string payerId = Request.Query["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = $"{Request.Scheme}://{Request.Host}/PayPal/PaymentWithPaypal?";
                    var guid = Guid.NewGuid().ToString();
                    var createdPayment = _payPalServiceold.CreatePayment(baseURI + "guid=" + guid);
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
                    var executedPayment = _payPalServiceold.ExecutePayment(payerId, HttpContext.Session.GetString(guid));
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
        public async Task<IActionResult> PaymentWithPaypal()
        {
            try
            {
                string payerId = Request.Query["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = $"{Request.Scheme}://{Request.Host}/Payment/PaymentWithPaypal?";
                    var guid = Guid.NewGuid().ToString();
                    string invoiceNumber = "INV-" + DateTime.Now.Ticks;

                    var createdPaymentJson = await _payPalService.CreatePaymentAsync(baseURI + "guid=" + guid, invoiceNumber);
                    var createdPayment = JsonDocument.Parse(createdPaymentJson);

                    string paypalRedirectUrl = null;
                    foreach (var link in createdPayment.RootElement.GetProperty("links").EnumerateArray())
                    {
                        if (link.GetProperty("rel").GetString() == "approval_url")
                        {
                            paypalRedirectUrl = link.GetProperty("href").GetString();
                            break;
                        }
                    }

                    HttpContext.Session.SetString(guid, createdPayment.RootElement.GetProperty("id").GetString());
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Query["guid"];
                    var executedPaymentJson = await _payPalService.ExecutePaymentAsync(payerId, HttpContext.Session.GetString(guid));
                    var executedPayment = JsonDocument.Parse(executedPaymentJson);

                    if (executedPayment.RootElement.GetProperty("state").GetString().ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (ex)
                return View("FailureView");
            }
            return View("SuccessView");
        }
    }
}


