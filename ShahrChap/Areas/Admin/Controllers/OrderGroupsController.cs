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
    public class OrderGroupsController : Controller
    {
        private UnitOfWork db = new UnitOfWork();

        // GET: Admin/Order_Groups
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListGroups()
        {
            return PartialView(db.Order_GroupsRepository.Get());
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
        public ActionResult Create([Bind(Include = "Order_GroupID,GroupTitle")] Order_Groups order_Groups)
        {
            if (ModelState.IsValid)
            {
                db.Order_GroupsRepository.Insert(order_Groups);
                db.Save();
                return PartialView("ListGroups", db.Order_GroupsRepository.Get());
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
            Order_Groups Order_Groups = db.Order_GroupsRepository.GetById(id);
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
        public ActionResult Edit([Bind(Include = "Order_GroupID,GroupTitle")] Order_Groups order_Groups)
        {
            if (ModelState.IsValid)
            {
                db.Order_GroupsRepository.Update(order_Groups);
                db.Save();
                return PartialView("ListGroups", db.Order_GroupsRepository.Get());
            }
            return View(order_Groups);
        }

        // GET: Admin/Order_Groups/Delete/5
        public void Delete(int? id)
        {
            var group = db.Order_GroupsRepository.GetById(id);
            db.Order_GroupsRepository.Delete(group);
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