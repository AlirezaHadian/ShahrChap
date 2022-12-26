using DataLayer;
using DataLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShahrChap.Controllers
{
    public class AboutUsController : Controller
    {
        UnitOfWork db = new UnitOfWork();
        // GET: AboutUs
        public ActionResult Index()
        {
            ViewBag.aboutUs = db.AboutUsRepository.Get().ToList();
            return View(db.AboutUsRepository.Get());
        }
        public ActionResult ContactUs()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult ContactUs(ContactUs contactUs)
        {
            return PartialView();
        }
    }
}