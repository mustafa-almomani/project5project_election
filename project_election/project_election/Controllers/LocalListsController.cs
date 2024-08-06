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
    public class LocalListsController : Controller
    {
        private electionEntities5 db = new electionEntities5();

        // GET: LocalLists
        public ActionResult Index()
        {
            return View(db.LocalLists.ToList());
        }

        // GET: LocalLists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocalList localList = db.LocalLists.Find(id);
            if (localList == null)
            {
                return HttpNotFound();
            }
            return View(localList);
        }

        // GET: LocalLists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LocalLists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ListName,NumberOfVotes,ElectionArea,Governorate")] LocalList localList)
        {
            if (ModelState.IsValid)
            {
                db.LocalLists.Add(localList);
                db.SaveChanges();
                return RedirectToAction("Create", "LocalListCandidates", new { listName = localList.ListName });
            }

            return View(localList);
        }

        // GET: LocalLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocalList localList = db.LocalLists.Find(id);
            if (localList == null)
            {
                return HttpNotFound();
            }
            return View(localList);
        }

        // POST: LocalLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ListName,NumberOfVotes,ElectionArea,Governorate")] LocalList localList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(localList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(localList);
        }

        // GET: LocalLists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocalList localList = db.LocalLists.Find(id);
            if (localList == null)
            {
                return HttpNotFound();
            }
            return View(localList);
        }

        // POST: LocalLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LocalList localList = db.LocalLists.Find(id);
            db.LocalLists.Remove(localList);
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
