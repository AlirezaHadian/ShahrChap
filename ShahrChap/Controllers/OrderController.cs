using DataLayer;
using DataLayer.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace ShahrChap.Controllers
{
    public class OrderController : Controller
    {
        UnitOfWork db = new UnitOfWork();
        public ActionResult LastOrderProducts()
        {
            return PartialView(db.Product_GroupsRepository.Get().Where(p => p.Order_GroupID != null && p.ST_GroupID == null).Select(p => p.Products).OrderByDescending(p => p.CreateDate).Take(12));
        }

        [Route("ShowOrderProduct/{id}")]
        public ActionResult ShowOrderProduct(int id)
        {
            var product = db.ProductsRepository.GetById(id);
            ViewBag.ProductFeatures = product.Product_Features.DistinctBy(f => f.FeatureID).Select(f => new ShowProductFeaturesViewModel()
            {
                FeatureTitle = f.Features.FeatureTitle,
                Values = db.Product_FeaturesRepository.Get().Where(fe => fe.FeatureID == f.FeatureID).Select(fe => fe.Value).ToList()
            }).ToList();
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost]
        public ActionResult SendOrder(int productId, bool desinged_check=false, string fronttext, string backtext, string description, string phonenumber, int count, HttpPostedFileBase[] files) 
        {
            if(desinged_check == false)
            {
                if(fronttext== null)
                {
                    ModelState.AddModelError("fronttext","متن روی محصول خود را وارد کنید");
                }
                if (phonenumber == null)
                {
                    ModelState.AddModelError("phonenumber", "شماره موبایل برای ارتباط با طراح را وارد کنید");
                }
            }
            Order_Details order = new Order_Details()
            {
                IsDesigned = desinged_check,
                OrderID = productId,
                BackText = backtext,
                FrontText = fronttext,
                Description = description,
                Count = count,
                SocialNumber = phonenumber,
                OrderDate = DateTime.Now
            };

            foreach (HttpPostedFileBase file in files)
            {
                //Checking file is available to save.  
                if (file != null)
                {
                    var FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var ServerSavePath = Path.Combine(Server.MapPath("/Images/OrderFiles/") + FileName);
                    //Save file to server folder  
                    file.SaveAs(ServerSavePath);
                    //assigning file uploaded status to ViewBag for showing message to user.  
                    ViewBag.UploadStatus = files.Count().ToString() + " files uploaded successfully.";
                }

            }
            //var product = db.ProductsRepository.GetById(id);
            //ViewBag.ProductFeatures = product.Product_Features.DistinctBy(f => f.FeatureID).Select(f => new ShowProductFeaturesViewModel()
            //{
            //    FeatureTitle = f.Features.FeatureTitle,
            //    Values = db.Product_FeaturesRepository.Get().Where(fe => fe.FeatureID == f.FeatureID).Select(fe => fe.Value).ToList()
            //}).ToList();
            //if (product == null)
            //{
            //    return HttpNotFound();
            //}
            return View();
        }

        [Route("ArchiveOrder")]
        public ActionResult ArchiveProduct(int pageId = 1, string title = "", int minPrice = 0, int maxPrice = 1000000, List<int> selectedGroups = null)
        {
            List<Products> products = db.Product_GroupsRepository.Get().Where(p => p.Order_GroupID != null && p.ST_GroupID == null).Select(p => p.Products).ToList();
            ViewBag.Groups = db.School_Tools_GroupsRepository.Get();
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
                    list.AddRange(db.Product_GroupsRepository.Get().Where(g => g.Order_GroupID == group && g.Order_GroupID != null && g.ST_GroupID == null).Select(g => g.Products).ToList());
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
                list = list.Where(p => p.Price >= minPrice && p.IsExist != false).ToList();
            }
            if (maxPrice > 0)
            {
                list = list.Where(p => p.Price <= maxPrice && p.IsExist != false).ToList();
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
            if ((list.Count() / take) < pageId)
            {
                pageId = 1;
            }
            return View(list.OrderByDescending(p => p.CreateDate).Skip(skip).Take(take).ToList());
        }
    }
}