using DataLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShahrChap.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/Home
        UnitOfWork db = new UnitOfWork();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Info()
        {
            var products = db.ProductsRepository.Get().Count();
            var users = db.UserRepository.Get().Count();
            return PartialView();
        }
    }
}