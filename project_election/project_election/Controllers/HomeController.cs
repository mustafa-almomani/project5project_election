using project_election.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace project_election.Controllers
{
    public class HomeController : Controller
    {
        private readonly electionEntities5 _context;
        private electionEntities5 DB=new electionEntities5();
        public HomeController()
        {
            _context = new electionEntities5();
        }
        public ActionResult Index()

        {

            var adsList = _context.Ads.Where(ad => ad.StatusOfAds == "approved").ToList();




            return View(adsList);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Useriformation()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Useriformation(string NationalNumber)
        {
            if (string.IsNullOrEmpty(NationalNumber))
            {
                ModelState.AddModelError("", "Please enter a valid National Number.");
                return View();
            }

            var userinf = DB.Users.FirstOrDefault(u => u.NationalNumber == NationalNumber);
            if (userinf == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View();
            }

            return View(userinf);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactFormModel model, [Bind(Include = "SenderName,SenderEmail,Message,SubmissionDate")] Contact contact)
        {
            if (ModelState.IsValid)
            {

                contact.SenderEmail = model.Email;
                contact.SenderName = model.Name;
                contact.Message = model.Message;
                contact.SubmissionDate = DateTime.Now;
                DB.Contacts.Add(contact);
                DB.SaveChanges();
                SendMessege(model.Name, model.Email, model.Message);
            }
            return View();
        }
        public void SendMessege(string name, string email, string message)
        {
            string ToEmail = ConfigurationManager.AppSettings["FromEmail"];
            string smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
            string smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;

            using (MailMessage mailMessage = new MailMessage())
            {
                string fromEmail = email.Trim();
                string fromName = name.Trim();
                string messageText = message.Trim();
                mailMessage.From = new MailAddress(fromEmail, fromName);
                mailMessage.To.Add(ToEmail);
                mailMessage.Subject = "Feedback";
                mailMessage.Body = messageText;
                mailMessage.IsBodyHtml = false;

                using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    smtpClient.EnableSsl = true;

                    smtpClient.Send(mailMessage);
                }
            }
        }
    }
}