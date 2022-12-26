using DataLayer;
using DataLayer.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Utilities;

namespace ShahrChap.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        private UnitOfWork db = new UnitOfWork();

        // GET: Admin/Products
        public ActionResult Index()
        {
            var products = db.ProductsRepository.Get(p=> p.IsOrder==true);
            return View(products.ToList());
        }

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.ProductsRepository.GetById(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.Groups = db.Product_GroupsRepository.Get(g=>g.IsOrder==true);
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,Title,Text,Price,ImageName,CreateDate,IsExist")] Products products, List<int> selectedGroups, HttpPostedFileBase imageProduct, string tags, int Count, bool IsTwoFace)
        {
            if (ModelState.IsValid)
            {
                if (selectedGroups == null)
                {
                    ViewBag.ErrorSelectedGroup = true;
                    ViewBag.Groups = db.Product_GroupsRepository.Get(g=>g.IsOrder==true);
                    return View(products);
                }
                products.ImageName = "images.jpg";
                if (imageProduct != null && imageProduct.IsImage())
                {
                    products.ImageName = Guid.NewGuid().ToString() + Path.GetExtension(imageProduct.FileName);
                    imageProduct.SaveAs(Server.MapPath("/Images/ProductImages/" + products.ImageName));
                    ImageResizer img = new ImageResizer();
                    img.Resize(Server.MapPath("/Images/ProductImages/" + products.ImageName),
                        Server.MapPath("/Images/ProductImages/Thumb/" + products.ImageName));
                }
                products.CreateDate = DateTime.Now;
                products.IsOrder = true;
                db.ProductsRepository.Insert(products);

                db.Product_AttributeRepository.Insert(new Product_Attribute()
                {
                    ProductID = products.ProductID,
                    Count = Count,
                    IsTwoFace = IsTwoFace
                });

                foreach (int selectedGroup in selectedGroups)
                {
                    db.Product_Selected_GroupsRepository.Insert(new Product_Selected_Groups()
                    {
                        ProductID = products.ProductID,
                        GroupID = selectedGroup
                    });
                }

                if (!string.IsNullOrEmpty(tags))
                {
                    string[] tag = tags.Split(',');
                    foreach (string t in tag)
                    {
                        db.TagsRepository.Insert(new Tags()
                        {
                            ProductID = products.ProductID,
                            Tag = t.Trim()
                        });
                    }
                }
                db.Save();
                return RedirectToAction("Index");
            }
            ViewBag.Groups = db.Product_GroupsRepository.Get(g=>g.IsOrder==true);
            return View(products);
        }


        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.ProductsRepository.GetById(id);
            if (products == null)
            {
                return HttpNotFound();
            }

            ViewBag.SelectedOrderGroups = products.Product_Selected_Groups.ToList();
            ViewBag.Groups = db.Product_GroupsRepository.Get(g=> g.IsOrder==true);
            ViewBag.Tags = string.Join(",", products.Tags.Select(t => t.Tag).ToList());
            return View(products);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,Title,Text,Price,ImageName,CreateDate,IsExist,IsOrder")] Products products, List<int> selectedGroups, HttpPostedFileBase imageProduct, string tags, int Count, bool IsTwoFace)
        {
            if (ModelState.IsValid)
            {
                if (imageProduct != null && imageProduct.IsImage())
                {
                    if (products.ImageName != "images.jpg")
                    {
                        System.IO.File.Delete(Server.MapPath("/Images/ProductImages/" + products.ImageName));
                        System.IO.File.Delete(Server.MapPath("/Images/ProductImages/Thumb/" + products.ImageName));
                    }

                    products.ImageName = Guid.NewGuid().ToString() + Path.GetExtension(imageProduct.FileName);
                    imageProduct.SaveAs(Server.MapPath("/Images/ProductImages/" + products.ImageName));
                    ImageResizer img = new ImageResizer();
                    img.Resize(Server.MapPath("/Images/ProductImages/" + products.ImageName),
                        Server.MapPath("/Images/ProductImages/Thumb/" + products.ImageName));
                }
                db.ProductsRepository.Update(products);

                db.Product_AttributeRepository.Update(new Product_Attribute()
                {
                    ProductID = products.ProductID,
                    Count = Count,
                    IsTwoFace = IsTwoFace
                });

                db.TagsRepository.Get().Where(t => t.ProductID== products.ProductID).ToList().ForEach(t => db.TagsRepository.Delete(t));
                if (!string.IsNullOrEmpty(tags))
                {
                    string[] tag = tags.Split(',');
                    foreach (string t in tag)
                    {
                        db.TagsRepository.Insert(new Tags()
                        {
                            ProductID = products.ProductID,
                            Tag = t.Trim()
                        });
                    }
                }

                db.Product_Selected_GroupsRepository.Get().Where(g => g.ProductID == products.ProductID).ToList().ForEach(g => db.Product_Selected_GroupsRepository.Delete(g));
                if (selectedGroups != null && selectedGroups.Any())
                {
                    foreach (int selectedGroup in selectedGroups) 
                    { 
                        db.Product_Selected_GroupsRepository.Insert(new Product_Selected_Groups()
                        {
                            ProductID = products.ProductID,
                            GroupID = selectedGroup
                        });
                    }
                }

                db.Save();
                return RedirectToAction("Index");
            }

            ViewBag.SelectedOrderGroups = selectedGroups;
            ViewBag.Groups = db.Product_GroupsRepository.Get(g=>g.IsOrder==true);
            ViewBag.Tags = tags;
            return View(products);
        }


        // POST: Admin/Products/Delete/5

        public void DeleteProduct(int id)
        {
            Products products = db.ProductsRepository.GetById(id);
            if (products.Product_Selected_Groups.Any())
            {
                foreach (var group in products.Product_Selected_Groups.Where(p => p.ProductID == products.ProductID).ToList())
                {
                    db.Product_Selected_GroupsRepository.Delete(group);
                }

            }
            if (products.Tags.Any())
            {
                foreach (var tag in products.Tags.Where(p => p.ProductID == products.ProductID).ToList())
                {
                    db.TagsRepository.Delete(tag);
                }
            }
            if (products.Product_Features.Any())
            {
                foreach (var features in products.Product_Features.Where(p => p.ProductID == products.ProductID).ToList())
                {
                    db.Product_FeaturesRepository.Delete(features);
                }
            }
            if (products.Product_Galleries.Any())
            {
                foreach (var gallery in products.Product_Galleries.Where(p => p.ProductID == products.ProductID).ToList())
                {
                    db.Product_GalleriesRepository.Delete(gallery);
                }
            }
            if(products.Product_Attribute.ProductID == products.ProductID)
            {
                db.Product_AttributeRepository.Delete(products.ProductID);
            }
            if (products.Order_Details.Any())
            {
                foreach(var detail in products.Order_Details.Where(o=> o.ProductID == products.ProductID))
                {
                    foreach(var files in detail.Order_Files.Where(o=> o.OT_ID == detail.OT_ID))
                    {
                        db.Order_FilesRepository.Delete(files);
                    }
                    db.Order_DetailsRepository.Delete(detail);                        
                }
            }
            if (products.Factor_Details.Any())
            {
                foreach(var factor_detail in products.Factor_Details.Where(d=> d.ProductID == products.ProductID))
                {
                    db.Factor_DetailsRepository.Delete(factor_detail);
                }
            }
            System.IO.File.Delete(Server.MapPath("/Images/ProductImages/" + products.ImageName));
            System.IO.File.Delete(Server.MapPath("/Images/ProductImages/Thumb/" + products.ImageName));
            db.ProductsRepository.Delete(products);
            db.Save();
        }
        #region Gallery
        public ActionResult Gallery(int id)
        {
            ViewBag.Galleries = db.Product_GalleriesRepository.Get().Where(p => p.ProductID == id).ToList();
            return View(new Product_Galleries()
            {
                ProductID = id
            });
        }

        [HttpPost]
        public ActionResult Gallery(Product_Galleries galleries, HttpPostedFileBase imgUp)
        {
            if (ModelState.IsValid)
            {
                if (imgUp != null && imgUp.IsImage())
                {
                    galleries.ImageName = Guid.NewGuid().ToString() + Path.GetExtension(imgUp.FileName);
                    imgUp.SaveAs(Server.MapPath("/Images/ProductImages/" + galleries.ImageName));
                    ImageResizer img = new ImageResizer();
                    img.Resize(Server.MapPath("/Images/ProductImages/" + galleries.ImageName),
                        Server.MapPath("/Images/ProductImages/Thumb/" + galleries.ImageName));
                    db.Product_GalleriesRepository.Insert(galleries);
                    db.Save();
                }
            }

            return RedirectToAction("Gallery", new { id = galleries.ProductID });
        }

        public ActionResult DeleteGallery(int id)
        {
            var gallery = db.Product_GalleriesRepository.GetById(id);

            System.IO.File.Delete(Server.MapPath("/Images/ProductImages/" + gallery.ImageName));
            System.IO.File.Delete(Server.MapPath("/Images/ProductImages/Thumb/" + gallery.ImageName));

            db.Product_GalleriesRepository.Delete(gallery);
            db.Save();
            return RedirectToAction("Gallery", new { id = gallery.ProductID });
        }

        #endregion
        #region Feature
        public ActionResult OrderFeaturs(int id)
        {

            ViewBag.Features = db.Product_FeaturesRepository.Get().Where(f => f.ProductID == id).ToList();
            ViewBag.FeatureID = new SelectList(db.FeaturesRepository.Get(), "FeatureID", "FeatureTitle");
            return View(new Product_Features()
            {
                ProductID = id
            });
        }

        [HttpPost]
        public ActionResult OrderFeaturs(Product_Features feature)
        {
            if (ModelState.IsValid)
            {
                db.Product_FeaturesRepository.Insert(feature);
                db.Save();
            }

            return RedirectToAction("OrderFeaturs", new { id = feature.ProductID });
        }

        public void DeleteOrderFeature(int id)
        {
            var feature = db.Product_FeaturesRepository.GetById(id);
            db.Product_FeaturesRepository.Delete(feature);
            db.Save();
        }

        #endregion
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