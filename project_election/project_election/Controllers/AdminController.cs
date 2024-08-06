using Newtonsoft.Json;
using project_election.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace project_election.Controllers
{
    public class AdminController : Controller
    {
        private readonly electionEntities5 _context;
        private electionEntities5 DB=new electionEntities5();

        public AdminController()
        {
            _context = new electionEntities5();
        }

        // GET: Admin
        public ActionResult Index()
        {
            var pendingAds = _context.Ads.Where(a => a.StatusOfAds == "pending").ToList();
            return View(pendingAds);
        }

        // POST: Admin/Approve/5
        [HttpPost]
        public ActionResult Approve(int id, string adminComment)
        {
            var ad = _context.Ads.Find(id);
            if (ad == null)
            {
                return HttpNotFound();
            }

            ad.StatusOfAds = "approved";
            ad.AdminComment = adminComment;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // POST: Admin/Reject/5
        [HttpPost]
        public ActionResult Reject(int id, string adminComment)
        {
            var ad = _context.Ads.Find(id);
            if (ad == null)
            {
                return HttpNotFound();
            }

            ad.StatusOfAds = "rejected";
            ad.AdminComment = adminComment;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult BulkAction(List<int> selectedIds, string action, FormCollection form)
        {
            if (selectedIds != null)
            {
                foreach (var id in selectedIds)
                {
                    var ad = _context.Ads.Find(id);
                    if (ad != null)
                    {
                        if (action == "Approve")
                        {
                            ad.StatusOfAds = "approved";
                        }
                        else if (action == "Reject")
                        {
                            ad.StatusOfAds = "rejected";
                        }

                        // Retrieve admin comment from form data
                        string adminCommentKey = $"adminComments[{id}]";
                        if (form.AllKeys.Contains(adminCommentKey))
                        {
                            ad.AdminComment = form[adminCommentKey];
                        }
                    }
                }
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        public ActionResult AddDate()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDate([Bind(Include = "startDateNominate,EndDateNominate,startDateOfElection,EndDateOfElection")] Date date)
        {
            if (ModelState.IsValid)
            {
                DB.Dates.Add(date);
                DB.SaveChanges();

            }
            return View();

        }
        public ActionResult LoginAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginAdmin(string Email, string Password)
        {
            var user = DB.Admins.FirstOrDefault(u => u.Email == Email);
            if (user != null)
            {
                if (user.PasswordHash == Password)
                {
                    TempData["LoggedUser"] = JsonConvert.SerializeObject(user);
                    return RedirectToAction("AddDate", new { id = user.AdminID });
                }
            }

            ViewBag.Message = "Invalid login attempt.";
            return View();
        }

    }
}