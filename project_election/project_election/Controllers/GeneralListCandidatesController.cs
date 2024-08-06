using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using project_election.Models;

namespace project_election.Controllers
{
    public class GeneralListCandidatesController : Controller
    {
        private electionEntities5 db = new electionEntities5();

        // GET: GeneralListCandidates
        public ActionResult Index()
        {
            //var generalListCandidates = db.GeneralListCandidates.Include(g => g.GeneralListing);
            return View(db.GeneralListCandidates.ToList());
        }

        // GET: GeneralListCandidates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GeneralListCandidate generalListCandidate = db.GeneralListCandidates.Find(id);
            if (generalListCandidate == null)
            {
                return HttpNotFound();
            }
            return View(generalListCandidate);
        }

        // GET: GeneralListCandidates/Create
    public ActionResult Create(string listName)
        {
            ViewBag.ListName = listName;
            return View(new List<GeneralListCandidate>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(List<GeneralListCandidate> candidates)
        {
            var listName = Request.QueryString["listName"];
            if (ModelState.IsValid)
            {
                foreach (var candidate in candidates)
                {
                    candidate.listname = (string) Session["listName"]; // تعيين اسم القائمة بدلاً من معرف القائمة
                    db.GeneralListCandidates.Add(candidate);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ListName = listName;
            return View(candidates);

         
        }

        // GET: GeneralListCandidates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GeneralListCandidate generalListCandidate = db.GeneralListCandidates.Find(id);
            if (generalListCandidate == null)
            {
                return HttpNotFound();
            }
            ViewBag.GeneralListingID = new SelectList(db.GeneralListings, "GeneralListingID", "Name", generalListCandidate.GeneralListingID);
            return View(generalListCandidate);
        }

        // POST: GeneralListCandidates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CandidateID,CandidateName,GeneralListingID")] GeneralListCandidate generalListCandidate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(generalListCandidate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GeneralListingID = new SelectList(db.GeneralListings, "GeneralListingID", "Name", generalListCandidate.listname);
            return View(generalListCandidate);
        }

        // GET: GeneralListCandidates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GeneralListCandidate generalListCandidate = db.GeneralListCandidates.Find(id);
            if (generalListCandidate == null)
            {
                return HttpNotFound();
            }
            return View(generalListCandidate);
        }

        // POST: GeneralListCandidates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GeneralListCandidate generalListCandidate = db.GeneralListCandidates.Find(id);
            db.GeneralListCandidates.Remove(generalListCandidate);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
