using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataLayer;
using DataLayer.Context;

namespace ShahrChap.Areas.Admin.Controllers
{
    public class FeaturesController : Controller
    {
        private UnitOfWork db = new UnitOfWork();

        // GET: Admin/Features
        public ActionResult Index()
        {
            return View(db.FeaturesRepository.Get());
        }

        // GET: Admin/Features/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Features features = db.FeaturesRepository.GetById(id);
            if (features == null)
            {
                return HttpNotFound();
            }
            return View(features);
        }

        // GET: Admin/Features/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Features/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FeatureID,FeatureTitle")] Features features)
        {
            if (ModelState.IsValid)
            {
                db.FeaturesRepository.Insert(features);
                db.Save();
                return RedirectToAction("Index");
            }

            return View(features);
        }

        // GET: Admin/Features/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Features features = db.FeaturesRepository.GetById(id);
            if (features == null)
            {
                return HttpNotFound();
            }
            return View(features);
        }

        // POST: Admin/Features/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FeatureID,FeatureTitle")] Features features)
        {
            if (ModelState.IsValid)
            {
                db.FeaturesRepository.Update(features);
                db.Save();
                return RedirectToAction("Index");
            }
            return View(features);
        }

        // GET: Admin/Features/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Features features = db.FeaturesRepository.GetById(id);
            if (features == null)
            {
                return HttpNotFound();
            }
            return View(features);
        }

        // POST: Admin/Features/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Features features = db.FeaturesRepository.GetById(id);
            db.FeaturesRepository.Delete(features);
            db.Save();
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
