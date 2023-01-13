using DataLayer;
using DataLayer.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;
using static System.Net.WebRequestMethods;

namespace ShahrChap.Controllers
{
    public class OrderController : Controller
    {
        UnitOfWork db = new UnitOfWork();
        public ActionResult LastOrderProducts()
        {
            return PartialView(db.ProductsRepository.Get().Where(p => p.IsOrder == true).OrderByDescending(p => p.CreateDate).Take(12));
        }

        public ActionResult BestSellersOrderProducts()
        {
            List<Factor_Details> list = new List<Factor_Details>();
            var products = db.Factor_DetailsRepository.Get(f => f.Factors.IsFinally == true).OrderBy(f => f.Count).ToList();
            list.AddRange(products.Distinct());
            return PartialView(list.Where(p => p.Products.IsOrder == true).Distinct());
        }

        [Route("ShowOrderProduct/{id}")]
        public ActionResult ShowOrderProduct(int id)
        {
            var product = db.ProductsRepository.GetById(id);
            ViewBag.OrderProductFeatures = product.Product_Features.DistinctBy(f => f.FeatureID).Select(f => new ShowProductFeaturesViewModel()
            {
                FeatureTitle = f.Features.FeatureTitle,
                Values = db.Product_FeaturesRepository.Get().Where(fe => fe.FeatureID == f.FeatureID && fe.ProductID == id).Select(fe => fe.Value).ToList()
            }).ToList();
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        [Route("SendOrder/{id}")]
        [Authorize]
        public ActionResult SendOrder(int id)
        {
            var product = db.ProductsRepository.GetById(id);
            ViewBag.IsTwoFace = db.Product_AttributeRepository.Get().Single(p => p.ProductID == id).IsTwoFace;
            return View(new Order_Details()
            {
                ProductID = id,
                Products = product
            });
        }

        [HttpPost]
        [Route("SendOrder/{id}")]
        [Authorize]
        public ActionResult SendOrder(int id, Order_Details sendOrder, HttpPostedFileBase[] files)
        {
            var product = db.ProductsRepository.GetById(id);
            ViewBag.IsTwoFace = db.Product_AttributeRepository.Get().Single(p => p.ProductID == id).IsTwoFace;
            if (sendOrder.IsDesigned == false)
            {
                if (sendOrder.FrontText == null)
                {
                    ModelState.AddModelError("FrontText", "وارد کردن قسمت روی اجباری می باشد");
                }
                if (product.Product_Attribute.IsTwoFace == true)
                {
                    if (sendOrder.BackText == null)
                    {
                        ModelState.AddModelError("BackText", "وارد کردن قسمت پشت اجباری می باشد");
                    }
                }
                if (sendOrder.SocialNumber == null)
                {
                    ModelState.AddModelError("SocialNumber", "وارد کردن شماره موبایل اجباری می باشد");
                }
                if (ModelState.IsValid)
                {
                    Order_Details order = new Order_Details()
                    {
                        IsDesigned = false,
                        ProductID = id,
                        BackText = sendOrder.BackText,
                        FrontText = sendOrder.FrontText,
                        Description = sendOrder.Description,
                        Count = sendOrder.Count,
                        SocialNumber = sendOrder.SocialNumber,
                        OrderDate = DateTime.Now,
                        IsDone = false
                    };
                    order.UserID = db.UserRepository.Get().SingleOrDefault(u => u.UserName == User.Identity.Name).UserID;
                    db.Order_DetailsRepository.Insert(order);
                        foreach (HttpPostedFileBase file in files)
                        {
                            if (file != null && file.ContentLength > 0)
                            {
                                var FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                                var ServerSavePath = Path.Combine(Server.MapPath("/Images/OrderFiles/") + FileName);
                                //Save file to server folder  
                                file.SaveAs(ServerSavePath);
                                //assigning file uploaded status to ViewBag for showing message to user.  
                                ViewBag.UploadStatus = files.Count().ToString() + " فایل با موفقیت آپلود شد.";

                                Order_Files orderFiles = new Order_Files()
                                {
                                    OT_ID = order.OT_ID,
                                    FileName = FileName
                                };
                                db.Order_FilesRepository.Insert(orderFiles);
                            }
                        }
                    db.Save();
                    ViewBag.OrderSuccess = true;
                    ShopController shop = new ShopController();
                    shop.Get(id,order.Count);
                    return View(new Order_Details()
                    {
                        ProductID = id,
                        Products = product,
                    });
                }
            }
            else
            {
                if (files != null)
                {
                    foreach (HttpPostedFileBase file in files)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            var FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var ServerSavePath = Path.Combine(Server.MapPath("/Images/OrderFiles/") + FileName);
                            //Save file to server folder  
                            file.SaveAs(ServerSavePath);
                            //assigning file uploaded status to ViewBag for showing message to user.  
                            ViewBag.UploadStatus = files.Count().ToString() + " فایل با موفقیت آپلود شد.";

                            Order_Details order = new Order_Details()
                            {
                                IsDesigned = true,
                                ProductID = id,
                                Count = sendOrder.Count,
                                OrderDate = DateTime.Now,
                                Description = sendOrder.Description,
                                SocialNumber = sendOrder.SocialNumber,
                                UserID = db.UserRepository.Get().SingleOrDefault(u => u.UserName == User.Identity.Name).UserID,
                                IsDone = false
                            };
                            db.Order_DetailsRepository.Insert(order);
                            Order_Files orderFiles = new Order_Files()
                            {
                                OT_ID = order.OT_ID,
                                FileName = FileName
                            };
                            db.Order_FilesRepository.Insert(orderFiles);
                            db.Save();
                            ViewBag.OrderSuccess = true;

                            ShopController shop = new ShopController();
                            shop.Get(id, order.Count);

                            return View(new Order_Details()
                            {
                                ProductID = id,
                                Products = product,
                            });
                        }
                        else
                        {
                            ViewBag.filevalidation = true;
                        }
                    }
                }
            }
            return View(sendOrder);
        }

        [Route("ArchiveOrder")]
        public ActionResult ArchiveProduct(int pageId = 1, string title = "", int minPrice = 0, int maxPrice = 1000000, List<int> selectedGroups = null)
        {
            List<Products> orderProducts = db.ProductsRepository.Get().Where(p => p.IsOrder == true).ToList();
            ViewBag.Groups = db.Product_GroupsRepository.Get(g => g.IsOrder == true);
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
                list.AddRange(orderProducts);
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
            if ((list.Count() / take) < pageId)
            {
                pageId = 1;
            }
            return View(list.OrderByDescending(p => p.CreateDate).Skip(skip).Take(take).ToList());
        }

    }
}