using DataLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataLayer;

namespace ShahrChap.Controllers
{
    public class SearchController : Controller
    {
        UnitOfWork db = new UnitOfWork();
        public ActionResult Index(string q)
        {
            if(q == null && q == "")
            {
                RedirectToAction("Index", "Home");
            }
            List<Products> list = new List<Products>();
            list.AddRange(db.TagsRepository.Get().Where(t => t.Tag == q).Select(t => t.Products).ToList());
            list.AddRange(db.ProductsRepository.Get().Where(p=> p.Title.Contains(q) || p.Title.Contains(q)).ToList());

            ViewBag.Search = q;
            return View(list.Distinct());
        }
    }
}