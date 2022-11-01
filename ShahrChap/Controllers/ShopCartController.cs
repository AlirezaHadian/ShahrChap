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
    public class ShopCartController : Controller
    {
        UnitOfWork db = new UnitOfWork();
        public ActionResult Index()
        {
            return View();
        }
        List<ShowCartViewModel> getListOrder()
        {
            List<ShowCartViewModel> list = new List<ShowCartViewModel>();

            if (Session["ShopCart"] != null)
            {
                List<ShopCartItem> listShop = Session["ShopCart"] as List<ShopCartItem>;
                foreach (var item in listShop)
                {
                    var product = db.ProductsRepository.Get().Where(p => p.ProductID == item.ProductID).Select(p => new
                    {
                        p.ImageName,
                        p.Price,
                        p.Title
                    }).Single();
                    list.Add(new ViewModels.ShowCartViewModel()
                    {
                        Count = item.Count,
                        ProductID = item.ProductID,
                        Title = product.Title,
                        Price = product.Price,
                        ImageName = product.ImageName,
                        Sum = item.Count * product.Price
                    });
                }
            }
            return list;
        }
        public ActionResult Cart()
        {
            int userId = GetUserId();
            List<User_Address> addressList = db.User_AddressRepository.Get().Where(u => u.UserID == userId).ToList();
            if (addressList == null)
            {
                return RedirectToAction("AddAddress", "UserPanel", new { Confirm = "NeedAddress" });
            }
            else
            {
                ViewBag.Address = addressList;
            }
            return PartialView(getListOrder());
        }

        public ActionResult CommandCart(int id, int count)
        {
            List<ShopCartItem> listShop = Session["ShopCart"] as List<ShopCartItem>;
            int index = listShop.FindIndex(p => p.ProductID == id);
            if(count == 0)
            {
                listShop.RemoveAt(index);
            }
            else
            {
                listShop[index].Count = count;
            }
            Session["ShopCart"] = listShop;
            return PartialView("Cart", getListOrder());
        }

        [Authorize]
        public ActionResult ConfirmBuy()
        {
            int userId = GetUserId();

            Factors factor = new Factors()
            {
                UserID = userId,
                Date = DateTime.Now,
                IsFinally = false
            };

            var listDetails = getListOrder();

            foreach(var item in listDetails)
            {
                db.Factor_DetailsRepository.Insert(new Factor_Details
                {
                    Count = item.Count,
                    FactorID = factor.FactorID,
                    Price = item.Price,
                    ProductID = item.ProductID
                });
            }
            db.Save();
            return null;
            //TO DO : Payment
        }

        public int GetUserId()
        {
            return db.UserRepository.Get().Single(u => u.UserName == User.Identity.Name).UserID;
        }
    }
}