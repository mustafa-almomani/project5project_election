using project_election.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace project_election.Controllers
{
    public class PaymentController : Controller
    {
        private readonly electionEntities5 _context;

        public PaymentController()
        {
            _context = new electionEntities5();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePayment(int AdID, string FirstName, string LastName, string PaymentMethod, DateTime PaymentDate, decimal Amount)
        {
            var ad = _context.Ads.Find(AdID);
            if (ad == null)
            {
                return HttpNotFound();
            }

            // Generate a unique TransactionID
            string transactionID = Guid.NewGuid().ToString();

            var payment = new Payment
            {
                Amount = Amount,
                PaymentDate = PaymentDate,
                PaymentMethod = PaymentMethod,
                TransactionID = transactionID,
                Status = "Pending"
            };

            _context.Payments.Add(payment);
            _context.SaveChanges();

            ad.PaymentID = payment.PaymentID;
            ad.StatusOfAds = "pending";
            _context.SaveChanges();

            // Read PayPal settings from Web.config
            string paypalUrl = WebConfigurationManager.AppSettings["PayPalSubmitUrl"];
            string business = WebConfigurationManager.AppSettings["PayPalBusinessEmail"];
            string item_name = "Advertisement Payment";
            string currency_code = "USD";
            string custom = payment.PaymentID.ToString();
            string returnUrl = Url.Action("Success", "Payment", null, Request.Url.Scheme); // Updated line
            string cancelUrl = WebConfigurationManager.AppSettings["PayPalCancelUrl"];
            string notifyUrl = WebConfigurationManager.AppSettings["PayPalNotifyUrl"];

            // Replace placeholders with actual values
            returnUrl = $"{returnUrl}?paymentId={payment.PaymentID}";
            cancelUrl = $"{cancelUrl}?paymentId={payment.PaymentID}";
            notifyUrl = $"{notifyUrl}?paymentId={payment.PaymentID}";

            string redirectUrl = $"{paypalUrl}?cmd=_xclick&business={business}&item_name={item_name}&amount={Amount}&currency_code={currency_code}&custom={custom}&return={returnUrl}&cancel_return={cancelUrl}&notify_url={notifyUrl}";

            return Redirect(redirectUrl);
        }

        public ActionResult Success()
        {
            ViewBag.Message = "Payment was successful.";
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public ActionResult IPN()
        {
            var formVals = new Dictionary<string, string>
            {
                { "cmd", "_notify-validate" }
            };

            string response;
            using (var client = new WebClient())
            {
                client.BaseAddress = "https://ipnpb.sandbox.paypal.com"; // Use sandbox for testing
                var req = Encoding.ASCII.GetBytes(Request.Form.ToString() + "&" + string.Join("&", formVals.Select(kvp => kvp.Key + "=" + kvp.Value)));
                var res = client.UploadData("/cgi-bin/webscr", "POST", req);
                response = Encoding.ASCII.GetString(res);
            }

            if (response.Equals("VERIFIED", StringComparison.OrdinalIgnoreCase))
            {
                var txnId = Request["txn_id"];
                var custom = Request["custom"];
                var payment = _context.Payments.SingleOrDefault(p => p.TransactionID == txnId);
                if (payment == null)
                {
                    var paymentId = int.Parse(custom);
                    payment = _context.Payments.Find(paymentId);

                    if (payment != null)
                    {
                        payment.Amount = Convert.ToDecimal(Request["mc_gross"]);
                        payment.PaymentDate = DateTime.Now;
                        payment.PaymentMethod = Request["payment_type"];
                        payment.TransactionID = txnId;
                        payment.Status = "Completed";

                        _context.SaveChanges();

                        var ad = _context.Ads.FirstOrDefault(a => a.PaymentID == paymentId);
                        if (ad != null)
                        {
                            ad.StatusOfAds = "Completed";
                            _context.SaveChanges();
                        }
                    }
                }
            }

            return new HttpStatusCodeResult(200);



        }

        //public ActionResult Success()
        //{
        //    ViewBag.Message = "Payment was successful.";
        //    return RedirectToAction("Index", "Home");
        //}

        public ActionResult Cancel()
        {
            ViewBag.Message = "Payment was cancelled.";
            return View();
        }
    }
}