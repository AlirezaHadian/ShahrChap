using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DataLayer;
using DataLayer.Context;

namespace ShahrChap.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        //private ShahrChap_DBEntities db = new ShahrChap_DBEntities();
        UnitOfWork db = new UnitOfWork();

        // GET: Admin/Users
        public ActionResult Index()
        {
            var users = db.UserRepository.Get();
            return View(users.ToList());
        }

        // GET: Admin/Users/Details/5
        public ActionResult Address(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<User_Address> userAddress = db.User_AddressRepository.Get().Where(u=> u.Users.UserID == id).ToList();
            ViewBag.Name = db.UserRepository.GetById(id).UserName;
            if (userAddress == null)
            {
                return HttpNotFound();
            }
            return View(userAddress);
        }

        // GET: Admin/Users/Create
        public ActionResult Create()
        {
            ViewBag.RoleID = new SelectList(db.RoleRepository.Get(), "RoleID", "RoleTitle");
            return View();
        }

        // POST: Admin/Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,RoleID,UserName,Email,ActiveCode,Phone,Password,IsEmailActive,IsPhoneActive,RegisterDate")] User user)
        {
            if (user.Email == null && user.Phone == null)
            {
                ModelState.AddModelError("Phone", "وارد کردن ایمیل یا شماره موبایل اجباری است.");
            }
            if (db.UserRepository.Get(u => u.Email == user.Email && u.Email != null).Any())
            {
                ModelState.AddModelError("Email", "ایمیل وارد شده تکراری میباشد");
            }
            if (db.UserRepository.Get(u => u.Phone == user.Phone && u.Phone != null).Any())
            {
                ModelState.AddModelError("Phone", "شماره موبایل وارد شده تکراری میباشد");
            }
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                user.RegisterDate = DateTime.Now;
                user.ActiveCode = Guid.NewGuid().ToString();
                user.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(user.Password, "MD5");
                db.UserRepository.Insert(user);
                db.Save();
                return RedirectToAction("Index");
            }

            ViewBag.RoleID = new SelectList(db.RoleRepository.Get(), "RoleID", "RoleTitle", user.RoleID);
            return View(user);
        }

        // GET: Admin/Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.UserRepository.GetById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleID = new SelectList(db.RoleRepository.Get(), "RoleID", "RoleTitle", user.RoleID);
            return View(user);
        }

        // POST: Admin/Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,RoleID,UserName,Email,ActiveCode,Phone,Password,IsEmailActive,IsPhoneActive,RegisterDate")] User user)
        {
            if(user.Email == null && user.Phone == null)
            {
                ModelState.AddModelError("Phone", "وارد کردن ایمیل یا شماره موبایل اجباری است.");
            }
            if (ModelState.IsValid)
            {
                db.UserRepository.Update(user);
                db.Save();
                return RedirectToAction("Index");
            }
            ViewBag.RoleID = new SelectList(db.RoleRepository.Get(), "RoleID", "RoleTitle", user.RoleID);
            return View(user);
        }

        // GET: Admin/Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.UserRepository.GetById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.UserRepository.GetById(id);
            if (user.User_Address.Any())
            {
                foreach(var userAddress in user.User_Address.Where(u=> u.UserID == user.UserID))
                {
                    db.User_AddressRepository.Delete(userAddress);
                }
            }
            db.UserRepository.Delete(user);
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
