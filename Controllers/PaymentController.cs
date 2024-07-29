using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;
using UniversityCateringSystem.Models;
using UniversityCateringSystem.Services;
using UniversityCateringSystem.Utils;

namespace UniversityCateringSystem.Controllers
{
    public class PaymentController : Controller
    {

        public readonly PayPalService _payPalService;
        private readonly IUserService _userService;

        public PaymentController( PayPalService payPalService, IUserService userService)
        {
          
            _payPalService = payPalService;
            this._userService = userService;
        }
        [Authorize]
        public IActionResult Index()
        {
          
            
            return View();
        }
        public async Task<ActionResult> Receipt(string invoiceNumber)
        {
           
          var invoice= await _userService.GetInvoiceByNumber(invoiceNumber);

            return View(invoice);
        }
       private object GetPayment(string guid, Invoice invoice)
        {
            string invoiceNumber = invoice.InvoiceNumber;
            string baseURI = $"{Request.Scheme}://{Request.Host}/Payment/PaymentWithPaypal?";
            
            string redirectUrl = baseURI + "guid=" + guid;
            var  paypalItems =invoice.CartLists.Select(item => new
            {
                name = item.ProductName,
                currency = item.Currency,
                price = item.Price.ToString("F2"), // Convert price to string with 2 decimal places
                quantity = item.Quantity.ToString(),
                sku = item.ProductId.ToString()
            }).ToArray();
            var payment = new
            {
                intent = "sale",
                payer = new { payment_method = "paypal" },
                transactions = new[]
                {
                new
                {
                    amount = new {
                        total = invoice.Amount.ToString("F2"),
                        currency = "GBP",
                        details = new
                        {
                            subtotal = invoice.Amount.ToString("F2"),




                        }
                    },
                    description = "Transaction description",
                    invoice_number = invoiceNumber,
                    item_list = new { items = paypalItems }
                }
            },
                redirect_urls = new { return_url = redirectUrl, cancel_url = $"{redirectUrl}&Cancel=true" }
            };
       
            
            
            return payment;
        }
      
       public async Task<IActionResult> PayAtCounter()
        {
            try
            {
                var guid = Guid.NewGuid().ToString();
                var appInvoice = CreateInvoiceAndMapCartItems(guid, true);
                await _userService.InsertInvoice(appInvoice);
            }
            catch(Exception ex) { 
                return BadRequest(ex.Message);
            }
            return RedirectToAction(nameof(Receipt), new { invoiceNumber = HttpContext.Session.GetString("INV") });
        }
        public async Task<IActionResult> PaymentWithPaypal()
        {
            string payerId = Request.Query["PayerID"];
            try
            {
              
                if (string.IsNullOrEmpty(payerId))
                {
                    var guid = Guid.NewGuid().ToString();
                    var appInvoice = CreateInvoiceAndMapCartItems(guid);
                    var recd = GetPayment(guid, appInvoice);

                    var createdPaymentJson = await _payPalService.CreatePaymentAsync( recd, appInvoice);
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
            return RedirectToAction("Receipt",new { invoiceNumber = HttpContext.Session.GetString("INV") } );
        }
        public Invoice CreateInvoiceAndMapCartItems(string ReceiptId, bool payLater=false )
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
           

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var cartTotal = cart.Sum(i => i.Quantity * i.Price);
            string invoiceNumber = "INV-" + DateTime.Now.Ticks;
            HttpContext.Session.SetString("INV", invoiceNumber);
            string baseURI = $"{Request.Scheme}://{Request.Host}/Payment/PaymentWithPaypal?";
            var guid = Guid.NewGuid().ToString();
            string redirectUrl = baseURI + "guid=" + guid;
            

            var cartItems = cart.Select(item => new CartList
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Price = item.Price,
                ImageUrl = item.ImageUrl,
                Quantity = item.Quantity,
                Currency = "GBP",
              
            }).ToList();
           
            var invoice = new Invoice
            {
                InvoiceNumber = invoiceNumber,
                Amount = cartTotal,
                UserId =Guid.Parse(UserId),
                InvoiceGuid= ReceiptId,
                Currency ="GBP",
                PaymentType=payLater? PaymentType.PayAtCounter : PaymentType.Paypal,
                TransactionStatus=TransactionStatus.Pending,
                redirectUrl=redirectUrl,
                CartLists= cartItems,
                

            };
            HttpContext.Session.Remove("Cart");
            return invoice;
        }
    }
}


