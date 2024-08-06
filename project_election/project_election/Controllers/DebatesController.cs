using project_election.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace project_election.Controllers
{
    public class DebatesController : Controller
    {
        private electionEntities5 db = new electionEntities5();

        // GET: Debates
        public ActionResult Index()
        {

            return View(db.Debates.Where(x => x.Status == true).ToList());
        }
        public ActionResult AdminDebates()
        {

            return View();
        }

        // GET: Debates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var debates = db.Debates.Find(id);
            if (debates == null)
            {
                return HttpNotFound();
            }
            return View(debates);
        }

        // GET: Debates/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Debates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DebateID,DebateTitle,DebateDate,Location,Candidate1ID,Candidate2ID,Description")] Debate debates)
        {
            if (ModelState.IsValid)
            {
                db.Debates.Add(debates);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(debates);
        }

        // GET: Debates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Debate debates = db.Debates.Find(id);
            if (debates == null)
            {
                return HttpNotFound();
            }
            return View(debates);
        }

        // POST: Debates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DebateID,DebateTitle,DebateDate,Location,Candidate1ID,Candidate2ID,Description")] Debate debates)
        {
            if (ModelState.IsValid)
            {
                db.Entry(debates).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(debates);
        }

        // GET: Debates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Debate debates = db.Debates.Find(id);
            if (debates == null)
            {
                return HttpNotFound();
            }
            return View(debates);
        }

        // POST: Debates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Debate debates = db.Debates.Find(id);
            db.Debates.Remove(debates);
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