using DataLayer;
using DataLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace ShahrChap.Controllers
{
    public class SchoolToolsController : Controller
    {
        UnitOfWork db = new UnitOfWork();
        public ActionResult ShowGroups()
        {
            var groups = db.Product_GroupsRepository.Get(g=> g.IsOrder==false);
            return PartialView(groups);
        }

        public ActionResult LastProduct()
        {
            return PartialView(db.ProductsRepository.Get().Where(p=> p.IsOrder==false).OrderByDescending(p => p.CreateDate).Take(12));
        }

        [Route("ShowProduct/{id}")]
        public ActionResult ShowProduct(int id)
        {
            var product = db.ProductsRepository.GetById(id);
            ViewBag.ProductFeatures = product.Product_Features.DistinctBy(f => f.FeatureID).Select(f => new ShowProductFeaturesViewModel() {
                FeatureTitle = f.Features.FeatureTitle,
                Values = db.Product_FeaturesRepository.Get().Where(fe => fe.FeatureID == f.FeatureID && fe.ProductID == id).Select(fe => fe.Value).ToList()
            }).ToList();
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [Route("Archive")]
        public ActionResult ArchiveProduct(int pageId=1, string title = "", int minPrice = 0, int maxPrice = 1000000, List<int> selectedGroups = null)
        {
            List<Products> products = db.ProductsRepository.Get().Where(p=> p.IsOrder==false).ToList();
            ViewBag.Groups = db.Product_GroupsRepository.Get(g=> g.IsOrder==false);
            ViewBag.productTitle = title;
            ViewBag.minPrice = minPrice;
            ViewBag.maxPrice = maxPrice;
            ViewBag.pageId = pageId;
            ViewBag.selectGroup = selectedGroups;
            List<Products> list = new List<Products>();
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
                list.AddRange(products);
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
            if((list.Count() / take) < pageId)
            {
                pageId = 1;
            }
            return View(list.OrderByDescending(p => p.CreateDate).Skip(skip).Take(take).ToList());
        }

    }
}