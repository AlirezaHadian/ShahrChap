using DataLayer;
using DataLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ShahrChap.Areas.Admin.Controllers
{
    public class Order_GroupsController : Controller
    {
        private UnitOfWork db = new UnitOfWork();

        // GET: Admin/Order_Groups
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListGroups()
        {
            return PartialView(db.Product_GroupsRepository.Get(g=> g.IsOrder==true));
        }


        // GET: Admin/Order_Groups/Create
        public ActionResult Create()
        {
            return PartialView();
        }

        // POST: Admin/Order_Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroupID,GroupTitle")] Product_Groups order_Groups)
        {
            if (ModelState.IsValid)
            {
                order_Groups.IsOrder = true;
                db.Product_GroupsRepository.Insert(order_Groups);
                db.Save();
                return PartialView("ListGroups", db.Product_GroupsRepository.Get(g=> g.IsOrder==true));
            }

            return PartialView(order_Groups);
        }

        // GET: Admin/Order_Groups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product_Groups Order_Groups = db.Product_GroupsRepository.GetById(id);
            if (Order_Groups == null)
            {
                return HttpNotFound();
            }
            return PartialView(Order_Groups);
        }

        // POST: Admin/Order_Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GroupID,GroupTitle")] Product_Groups order_Groups)
        {
            if (ModelState.IsValid)
            {
                order_Groups.IsOrder = true;
                db.Product_GroupsRepository.Update(order_Groups);
                db.Save();
                return PartialView("ListGroups", db.Product_GroupsRepository.Get(g=> g.IsOrder==true));
            }
            return View(order_Groups);
        }

        // GET: Admin/Order_Groups/Delete/5
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