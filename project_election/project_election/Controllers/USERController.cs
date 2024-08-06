using Newtonsoft.Json;
using project_election.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace project_election.Controllers
{
    public class USERController : Controller
    {
        private electionEntities5 DB = new electionEntities5();

        // GET: USER
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Login(User user)
        {
            try
            {
                if (user == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    return View();
                }

                var existingUser = DB.Users.FirstOrDefault(u => u.NationalNumber == user.NationalNumber);

                if (existingUser == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    return View();
                }

                if (existingUser.Password == "password")
                {
                    string newPassword = GenerateRandomPassword();
                    existingUser.Password = newPassword;
                    DB.SaveChanges();

                    SendConfirmationEmail(existingUser.Email, newPassword);

                    ViewBag.Emailsent = "The code has been sent to your Email";
                    return RedirectToAction("LoginUser", new { NationalNumber = user.NationalNumber });
                }
                else
                {
                    ViewBag.NationalId = existingUser.NationalNumber;
                    return RedirectToAction("LoginUser", new { NationalNumber = user.NationalNumber });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing your request. Please try again later.");
                Console.WriteLine("Exception message: " + ex.Message);
            }

            return View();
        }
        public ActionResult LoginUser(string NationalNumber, string Email, string password)
        {

            var user = DB.Users.FirstOrDefault(u => u.NationalNumber == NationalNumber);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View();
            }

            if (ModelState.IsValid)
            {
                if (Email == user.Email && password == user.Password)
                {
                    TempData["LoggedUser"] = JsonConvert.SerializeObject(user);
                    return RedirectToAction("TypeOfElection", new { id = user.ID });
                }

                ModelState.AddModelError("", "Invalid login attempt.");
            }

            return View();
        }

        private string GenerateRandomPassword()
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            int length = 8;
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        private void SendConfirmationEmail(string toEmail, string confirmationCode)
        {
            string fromEmail = System.Configuration.ConfigurationManager.AppSettings["FromEmail"];
            string smtpUsername = System.Configuration.ConfigurationManager.AppSettings["SmtpUsername"];
            string smtpPassword = System.Configuration.ConfigurationManager.AppSettings["SmtpPassword"];

            string subjectText = "Your Confirmation Code";
            string messageText = $"Your confirmation code is {confirmationCode}";

            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;

            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(fromEmail);
                mailMessage.To.Add(toEmail);
                mailMessage.Subject = subjectText;
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
        public ActionResult TypeOfElection(int? id)
        {
            var user = DB.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            if (user.LocalElections is false)
            {

                ViewBag.LocalElectionsPath = "LocalElections";
            }
            else
            {
                ViewBag.LocalElectionsPath = null;
            }
            if (user.PartyElections is false)
            {

                ViewBag.PartyElections = "PartyElections";
            }
            else
            {
                ViewBag.PartyElections = null;

            }

            return View();
        }

        public ActionResult LocalElections()
        {
            if (TempData["LoggedUser"] != null)
            {
                var userJson = TempData["LoggedUser"].ToString();
                var user = JsonConvert.DeserializeObject<User>(userJson);
                var localLists = DB.LocalLists.ToList();

                var areaOfElection = user.ElectionArea;

                List<LocalList> filterList = new List<LocalList>();

                foreach (var item in localLists)
                {
                    if (item.ElectionArea == areaOfElection)
                    {
                        filterList.Add(item);
                    }
                }
                var viewModel = new LocalListViewModel

                {
                    LocalLists = filterList,
                    LoggedInUser = user
                };
                TempData.Keep("LoggedUser");
                return View(viewModel);
            }
            return RedirectToAction("TypeOfElection", new { id = User.Identity.Name });
        }

        [HttpPost]
        public JsonResult IncrementVoteLocal(int listId, int[] candidateIds)
        {
            try
            {
                var list = DB.LocalLists.Find(listId);
                if (list == null)
                {
                    return Json(new { success = false, message = "List not found." });
                }

                var userJson = TempData["LoggedUser"]?.ToString();
                if (string.IsNullOrEmpty(userJson))
                {
                    return Json(new { success = false, message = "User not found." });
                }

                var user = JsonConvert.DeserializeObject<User>(userJson);
                if (user == null)
                {
                    return Json(new { success = false, message = "Invalid user data." });
                }

                var existingUser = DB.Users.Find(user.ID);
                if (existingUser == null)
                {
                    return Json(new { success = false, message = "User not found in database." });
                }

                existingUser.LocalElections = true;
                list.NumberOfVotes += 1;
                if (candidateIds != null)
                {

                    foreach (var candidateId in candidateIds)
                    {
                        var candidate = DB.LocalListCandidates.Find(candidateId);
                        if (candidate != null && candidate.LocalListingID == listId)
                        {
                            candidate.NumberOfVotesCandidate += 1;
                        }
                    }
                }
                DB.SaveChanges();


                string redirectUrl = Url.Action("TypeOfElection", new { id = user.ID });

                return Json(new { success = true, redirectUrl = redirectUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return Json(new { success = false, message = "An error occurred while processing your request." });
            }
        }




        public ActionResult PartyElections()
        {
            if (TempData["LoggedUser"] != null)
            {
                var userJson = TempData["LoggedUser"].ToString();
                var user = JsonConvert.DeserializeObject<User>(userJson);
                var party = DB.GeneralListings.ToList();
                ViewBag.User = user;
                TempData.Keep("LoggedUser");
                return View(party);
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        public JsonResult IncrementVote(int id)
        {
            try
            {
                var vote = DB.GeneralListings.Find(id);
                if (vote == null)
                {
                    return Json(new { success = false, message = "Vote not found." });
                }

                var userJson = TempData["LoggedUser"]?.ToString();
                if (string.IsNullOrEmpty(userJson))
                {
                    return Json(new { success = false, message = "User not found." });
                }

                var user = JsonConvert.DeserializeObject<User>(userJson);
                if (user == null)
                {
                    return Json(new { success = false, message = "Invalid user data." });
                }

                var existingUser = DB.Users.Find(user.ID);
                if (existingUser == null)
                {
                    return Json(new { success = false, message = "User not found in database." });
                }

                existingUser.PartyElections = true;
                vote.NumberOfVotes += 1;

                DB.SaveChanges();
                string redirectUrl = Url.Action("TypeOfElection", new { id = user.ID });

                return Json(new { success = true, redirectUrl = redirectUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return Json(new { success = false, message = "An error occurred while processing your request." });
            }
        }
        public ActionResult WhiteCard()
        {
            var userJson = TempData["LoggedUser"]?.ToString();
            if (string.IsNullOrEmpty(userJson))
            {
                return Json(new { success = false, message = "User not found." });
            }

            var user = JsonConvert.DeserializeObject<User>(userJson);
            if (user == null)
            {
                return Json(new { success = false, message = "Invalid user data." });
            }

            var existingUser = DB.Users.Find(user.ID);
            if (existingUser == null)
            {
                return Json(new { success = false, message = "User not found in database." });
            }


            existingUser.WhitePaperPartyElections = true;

            DB.SaveChanges();


            string redirectUrl = Url.Action("TypeOfElection", new { id = user.ID });

            return Json(new { success = true, redirectUrl = redirectUrl });
        }

    }
}