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
    public class AboutUsController : Controller
    {
        private UnitOfWork db = new UnitOfWork();

        // GET: Admin/AboutUs
        public ActionResult Index()
        {
            return View(db.AboutUsRepository.Get());
        }

        // GET: Admin/AboutUs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AboutUs aboutUs = db.AboutUsRepository.GetById(id);
            if (aboutUs == null)
            {
                return HttpNotFound();
            }
            return View(aboutUs);
        }

        // GET: Admin/AboutUs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/AboutUs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AboutUsID,Text")] AboutUs aboutUs)
        {
            if (ModelState.IsValid)
            {
                db.AboutUsRepository.Insert(aboutUs);
                db.Save();
                return RedirectToAction("Index");
            }

            return View(aboutUs);
        }

        // GET: Admin/AboutUs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AboutUs aboutUs = db.AboutUsRepository.GetById(id);
            if (aboutUs == null)
            {
                return HttpNotFound();
            }
            return View(aboutUs);
        }

        // POST: Admin/AboutUs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AboutUsID,Text")] AboutUs aboutUs)
        {
            if (ModelState.IsValid)
            {
                db.AboutUsRepository.Update(aboutUs);
                db.Save();
                return RedirectToAction("Index");
            }
            return View(aboutUs);
        }

        // GET: Admin/AboutUs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AboutUs aboutUs = db.AboutUsRepository.GetById(id);
            if (aboutUs == null)
            {
                return HttpNotFound();
            }
            return View(aboutUs);
        }

        // POST: Admin/AboutUs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AboutUs aboutUs = db.AboutUsRepository.GetById(id);
            db.AboutUsRepository.Delete(aboutUs);
            db.Save();
            return RedirectToAction("Index");
        }

        #region ContactUsInfo
        public ActionResult ContactUsInfo()
        {
            return PartialView(db.ContactUsInfoRepository.Get());
        }
        public ActionResult CreateContactUsInfo()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateContactUsInfo(ContactUsInfo info)
        {
            if (ModelState.IsValid)
            {
                db.ContactUsInfoRepository.Insert(info);
                db.Save();
                return RedirectToAction("Index");
            }

            return View(info);
        }

        public ActionResult EditContactUsInfo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUsInfo info = db.ContactUsInfoRepository.GetById(id);
            if (info == null)
            {
                return HttpNotFound();
            }
            return View(info);
        }

        // POST: Admin/AboutUs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditContactUsInfo([Bind(Include = "AboutUsID,Text")] ContactUsInfo info)
        {
            if (ModelState.IsValid)
            {
                db.ContactUsInfoRepository.Update(info);
                db.Save();
                return RedirectToAction("Index");
            }
            return View(info);
        }

        // GET: Admin/AboutUs/Delete/5
        public ActionResult DeleteInfo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUsInfo info = db.ContactUsInfoRepository.GetById(id);
            if (info == null)
            {
                return HttpNotFound();
            }
            return View(info);
        }

        // POST: Admin/AboutUs/DeleteContactUsInfo/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteInfo(int id)
        {
            ContactUsInfo info = db.ContactUsInfoRepository.GetById(id);
            db.ContactUsInfoRepository.Delete(info);
            db.Save();
            return RedirectToAction("Index");
        }
        #endregion
    }
}
