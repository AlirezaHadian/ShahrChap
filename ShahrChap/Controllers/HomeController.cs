using DataLayer;
using DataLayer.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Utilities;

namespace ShahrChap.Controllers
{
    public class HomeController : Controller
    {
        UnitOfWork db = new UnitOfWork();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        [ChildActionOnly]
        public ActionResult Slider()
        {
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0 , 0 ,0);
            return PartialView(db.SliderRepository.Get().Where(s=> s.IsActive && s.StartDate<= dt && s.EndDate >= dt));
        }

    }
}