using DataLayer;
using DataLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
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
            if (!User.Identity.IsAuthenticated)
            {
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
                        list.Add(new ShowCartViewModel()
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
            }
            else
            {
                int userId = db.UserRepository.Get().Single(u => u.UserName == User.Identity.Name).UserID;
                List<ShopCart> shop = db.ShopCartRepository.Get().Where(f => f.UserID == userId).ToList();
                foreach (var item in shop)
                {
                    var product = db.ProductsRepository.Get().Where(p => p.ProductID == item.ProductID).Select(p => new
                    {
                        p.ImageName,
                        p.Title
                    }).Single();
                    list.Add(new ShowCartViewModel
                    {
                        Count = item.Count,
                        ProductID = item.ProductID,
                        ImageName = product.ImageName,
                        Price = item.Price,
                        Sum = item.Sum,
                        Title = product.Title
                    });

                }
            }
            return list;
        }
        public ActionResult Cart()
        {
            if (User.Identity.IsAuthenticated)
            {
                int userId = GetUserId();
                ViewBag.id = userId;
                List<User_Address> addressList = db.User_AddressRepository.Get().Where(u => u.UserID == userId).ToList();

                if (addressList == null)
                {
                    return RedirectToAction("AddAddress", "UserPanel", new { Confirm = "NeedAddress" });
                }
                else
                {
                    TempData["AddressList"] = addressList;
                    ViewBag.Address = addressList;
                }
            }

            return PartialView(getListOrder());
        }

        public ActionResult CommandCart(int id, int count)
        {
            if (User.Identity.IsAuthenticated)
            {
                int userId = GetUserId();
                List<ShopCart> shopList = db.ShopCartRepository.Get().Where(f => f.UserID == userId).ToList();
                int index = shopList.FindIndex(p => p.ProductID == id);
                if (count == 0)
                {
                    shopList.RemoveAt(index);
                }
                else
                {
                    shopList[index].Count = count;
                }
                db.Save();
            }
            else
            {
                List<ShopCartItem> sessionList = Session["ShopCart"] as List<ShopCartItem>;
                int index = sessionList.FindIndex(p => p.ProductID == id);
                if (count == 0)
                {
                    sessionList.RemoveAt(index);
                }
                else
                {
                    sessionList[index].Count = count;
                }
                Session["ShopCart"] = sessionList;
            }            
            return PartialView("Cart", getListOrder());
        }

        [Authorize]
        [HttpPost]
        public ActionResult ConfirmBuy()
        {
            int userId = GetUserId();
            var listDetails = getListOrder();
            if (listDetails.Count > 0)
            {
                Factors factor = new Factors()
                {
                    UserID = userId,
                    Date = DateTime.Now,
                    IsFinally = false
                };
                factor.TotalPrice = listDetails.Sum(t => t.Sum);
                db.FactorsRepository.Insert(factor);

                foreach (var item in listDetails)
                {
                    Factor_Details factor_Details = new Factor_Details()
                    {
                        Count = item.Count,
                        FactorID = factor.FactorID,
                        Price = item.Price,
                        ProductID = item.ProductID,
                        Sum = item.Count * item.Price
                    };
                    db.Factor_DetailsRepository.Insert(factor_Details);
                }
                db.Save();
                return RedirectToAction("SelectAddress", new { factorId = factor.FactorID });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult SelectAddress(int factorId)
        {
            ViewBag.FactorId = db.FactorsRepository.GetById(factorId).FactorID;
            int userId = GetUserId();
            List<User_Address> addressList = db.User_AddressRepository.Get().Where(u => u.UserID == userId).ToList();
            ViewBag.Address = addressList;
            return View(addressList);
        }
        [HttpPost]
        public ActionResult SelectAddress(int factorId, int address = 0)
        {
            int userId = GetUserId();
            int addressCount = db.UserRepository.GetById(userId).User_Address.Count();
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            else if (addressCount == 0)
            {
                return RedirectToAction("Index", "UserPanel");
            }
            else
            {

                if (address != 0)
                {
                    var factor = db.FactorsRepository.GetById(factorId);
                    factor.AddressID = address;
                    db.Save();
                    var totalPrice = db.FactorsRepository.Get().Single(f => f.FactorID == factor.FactorID).TotalPrice;
                    System.Net.ServicePointManager.Expect100Continue = false;
                    Zarinpal.PaymentGatewayImplementationServicePortTypeClient zp = new Zarinpal.PaymentGatewayImplementationServicePortTypeClient();
                    string Authority;

                    int Status = zp.PaymentRequest("YOUR-ZARINPAL-MERCHANT-CODE", totalPrice, "تست درگاه زرین پال در شهر چاپ", "ali.h.reza8@gmail.com", "09397673794", "https://localhost:44345/ShopCart/Verify/" + factor.FactorID, out Authority);

                    if (Status == 100)
                    {
                        //Response.Redirect("https://www.zarinpal.com/pg/StartPay/" + Authority);
                        Response.Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + Authority);
                    }
                    else
                    {
                        Response.Write("error: " + Status);
                    }
                }
                else
                {
                    ViewBag.ValidateAddress = true;
                }
            }
            List<User_Address> addressList = db.User_AddressRepository.Get().Where(u => u.UserID == userId).ToList();
            return View(addressList);
        }
        public ActionResult Verify(int id)
        {
            var factor = db.FactorsRepository.GetById(id);
            //var factorDetail = db.Factor_DetailsRepository.Get().Single(f => f.FactorID == factor.FactorID);

            if (Request.QueryString["Status"] != "" && Request.QueryString["Status"] != null && Request.QueryString["Authority"] != "" && Request.QueryString["Authority"] != null)
            {
                if (Request.QueryString["Status"].ToString().Equals("OK"))
                {
                    int Amount = factor.TotalPrice;
                    long RefID;
                    System.Net.ServicePointManager.Expect100Continue = false;
                    Zarinpal.PaymentGatewayImplementationServicePortTypeClient zp = new Zarinpal.PaymentGatewayImplementationServicePortTypeClient();

                    int Status = zp.PaymentVerification("YOUR-ZARINPAL-MERCHANT-CODE", Request.QueryString["Authority"].ToString(), Amount, out RefID);

                    if (Status == 100)
                    {
                        factor.IsFinally = true;
                        db.Save();
                        ViewBag.IsSuccess = true;
                        ViewBag.RefId = RefID;
                        Session.Remove("ShopCart");
                    }
                    else
                    {
                        ViewBag.IsSuccess = false;
                        ViewBag.Status = Status;
                    }

                }
                else
                {
                    ViewBag.IsSuccess = false;
                    //Response.Write("Error! Authority: " + Request.QueryString["Authority"].ToString() + " Status: " + Request.QueryString["Status"].ToString());
                }
            }
            else
            {
                Response.Write("Invalid Input");
            }
            return View();
        }
        public int GetUserId()
        {
            return db.UserRepository.Get().Single(u => u.UserName == User.Identity.Name).UserID;
        }
    }
}