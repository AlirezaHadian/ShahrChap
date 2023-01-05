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
        public ActionResult Index(string q, int pageId = 1, string title = "", int minPrice = 0, int maxPrice = 1000000, List<int> selectedGroups = null)
        {
            if(q == "")
            {
                return RedirectToAction("Index", "Home");
            }
            List<Products> list = new List<Products>();
            list.AddRange(db.TagsRepository.Get().Where(t => t.Tag == q).Select(t => t.Products).ToList());
            list.AddRange(db.ProductsRepository.Get().Where(p=> p.Title.Contains(q) || p.Title.Contains(q)).ToList());
            ViewBag.Search = q;

            ViewBag.Groups = db.Product_GroupsRepository.Get();
            ViewBag.productTitle = title;
            ViewBag.minPrice = minPrice;
            ViewBag.maxPrice = maxPrice;
            ViewBag.pageId = pageId;
            ViewBag.selectGroup = selectedGroups;
            if (selectedGroups != null && selectedGroups.Any())
            {
                foreach (int group in selectedGroups)
                {
                    list.AddRange(db.Product_Selected_GroupsRepository.Get().Where(g => g.GroupID == group).Select(g => g.Products).ToList());
                }
                list = list.Distinct().ToList();
            }
            else
            {
                list.AddRange(list);
            }

            if (title != "")
            {
                list = list.Where(p => p.Title.Contains(title)).ToList();
            }
            if (minPrice > 0)
            {
                list = list.Where(p => p.Price >= minPrice && p.IsExist == true).ToList();
            }
            if (maxPrice < 1000000)
            {
                list = list.Where(p => p.Price <= maxPrice && p.IsExist == true).ToList();
            }
            list = list.Distinct().ToList();
            //Pagging
            int take = 9;
            int skip = (pageId - 1) * take;
            if (list.Count() % take == 0)
            {
                ViewBag.PageCount = (list.Count() / take);
            }
            else
            {
                ViewBag.PageCount = (list.Count() / take) + 1;
            }
            ViewBag.pageId = pageId;
            if ((list.Count() / take) < pageId)
            {
                pageId = 1;
            }
            return View(list.OrderByDescending(p => p.CreateDate).Skip(skip).Take(take).ToList());


            //return View(list.Distinct());
        }
    }
}