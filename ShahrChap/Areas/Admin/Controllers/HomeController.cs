using DataLayer.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

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
            ViewBag.products = db.ProductsRepository.Get().Count();
            ViewBag.users = db.UserRepository.Get().Count();
            ViewBag.factors = db.FactorsRepository.Get(f=> f.IsFinally==true).Count();
            ViewBag.orders = db.Order_DetailsRepository.Get().Count();
            return PartialView();
        }

        public ActionResult VisitSite()
        {
            DateTime dtNow = DateTime.Now.Date;
            DateTime dtYesterday = dtNow.AddDays(-1);
            DateTime startMonth = new DateTime(dtNow.Year, dtNow.Month, 1);
            DateTime endMonth = startMonth.AddMonths(1).AddDays(-1);
            DateTime startLastMonth = startMonth.AddMonths(-1);
            DateTime endLastMonth = startMonth.AddDays(-1);


            VisitSiteViewModel visit = new VisitSiteViewModel();
            visit.VisitMonth = db.SiteVisitRepository.Get(v=> v.Date>= startMonth && v.Date <= endMonth).Count();
            visit.VisitPrevMonth = db.SiteVisitRepository.Get(v => v.Date >= startLastMonth && v.Date <= endLastMonth).Count();
            visit.VisitToday = db.SiteVisitRepository.Get().Count(v => v.Date == dtNow);
            visit.VisitYesterday = db.SiteVisitRepository.Get().Count(v => v.Date == dtYesterday);
            visit.TotalVisit = db.SiteVisitRepository.Get().Count();
            visit.Online = int.Parse(HttpContext.Application["Online"].ToString());

            return PartialView(visit);
        }
        public ActionResult ProgressInfo()
        {
            int ActiveatedAccount = db.UserRepository.Get(u => u.IsPhoneActive == true || u.IsEmailActive == true).Count();
            int Account = db.UserRepository.Get().Count();
            ViewBag.Acitve = ((ActiveatedAccount / Account) * 100)+"%";
            int FinishedDesign = db.Order_DetailsRepository.Get(o=> o.IsDesigned== false && o.IsDone == true).Count();
            int Design = db.Order_DetailsRepository.Get(o=> o.IsDesigned== false).Count();
            ViewBag.Done = ((FinishedDesign / Design) * 100)+"%";

            return PartialView();
        }
    }
}