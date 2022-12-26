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
    public class SchoolTools_GroupsController : Controller
    {
        private UnitOfWork db = new UnitOfWork();

        // GET: Admin/SchoolTools_Groups
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListGroups()
        {
            return PartialView(db.Product_GroupsRepository.Get(g=> g.IsOrder==false));
        }

       
        // GET: Admin/SchoolTools_Groups/Create
        public ActionResult Create()
        {
            return PartialView();
        }

        // POST: Admin/SchoolTools_Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroupID,GroupTitle")] Product_Groups school_Tools_Groups)
        {
            if (ModelState.IsValid)
            {
                school_Tools_Groups.IsOrder = false;
                db.Product_GroupsRepository.Insert(school_Tools_Groups);
                db.Save();
                return PartialView("ListGroups", db.Product_GroupsRepository.Get(g => g.IsOrder == false));
            }

            return PartialView(school_Tools_Groups);
        }

        // GET: Admin/SchoolTools_Groups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product_Groups schoolTools_Groups = db.Product_GroupsRepository.GetById(id);
            if (schoolTools_Groups == null)
            {
                return HttpNotFound();
            }
            return PartialView(schoolTools_Groups);
        }

        // POST: Admin/SchoolTools_Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GroupID,GroupTitle")] Product_Groups school_Tools_Groups)
        {
            if (ModelState.IsValid)
            {
                school_Tools_Groups.IsOrder = false;
                db.Product_GroupsRepository.Update(school_Tools_Groups);
                db.Save();
                return PartialView("ListGroups", db.Product_GroupsRepository.Get(g => g.IsOrder == false));
            }
            return View(school_Tools_Groups);
        }

        // GET: Admin/SchoolTools_Groups/Delete/5
        public void Delete(int? id)
        {
            var group = db.Product_GroupsRepository.GetById(id);
            db.Product_GroupsRepository.Delete(group);
            db.Save();
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
