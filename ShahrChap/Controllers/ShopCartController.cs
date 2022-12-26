using DataLayer;
using DataLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    ViewBag.Address = addressList;
                }
            }

            return PartialView(getListOrder());
        }

        public ActionResult CommandCart(int id, int count)
        {
            List<ShopCartItem> listShop = Session["ShopCart"] as List<ShopCartItem>;
            int index = listShop.FindIndex(p => p.ProductID == id);
            if (count == 0)
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
        [HttpPost, ActionName("Index")]
        public ActionResult ConfirmBuy(int address)
        {
            int userId = GetUserId();
            var listDetails = getListOrder();
            if (address != 0)
            {
                Factors factor = new Factors()
                {
                    UserID = userId,
                    Date = DateTime.Now,
                    IsFinally = false,
                    AddressID = address
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


                var totalPrice = db.FactorsRepository.Get().Single(f => f.FactorID == factor.FactorID).TotalPrice;

                System.Net.ServicePointManager.Expect100Continue = false;
                Zarinpal.PaymentGatewayImplementationServicePortTypeClient zp = new Zarinpal.PaymentGatewayImplementationServicePortTypeClient();
                string Authority;

                int Status = zp.PaymentRequest("YOUR-ZARINPAL-MERCHANT-CODE", totalPrice, "تست درگاه زرین پال در شهر چاپ", "ali.h.reza8@gmail.com", "09397673794", "http://localhost:44345/ShopCart/Verify" + factor.FactorID, out Authority);

                if (Status == 100)
                {
                    Response.Redirect("https://www.zarinpal.com/pg/StartPay/" + Authority);
                    Response.Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + Authority);
                }
                else
                {
                    Response.Write("error: " + Status);
                }
            }
            else
            {
                ViewBag.AddressValidate = true;
            }
            ViewBag.id = userId;
            List<User_Address> addressList = db.User_AddressRepository.Get().Where(u => u.UserID == userId).ToList();
            return View();
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
                        ViewBag.IsSuccess = true;
                        ViewBag.RefId = RefID;
                    }
                    else
                    {
                        ViewBag.Status = Status;
                    }

                }
                else
                {
                    Response.Write("Error! Authority: " + Request.QueryString["Authority"].ToString() + " Status: " + Request.QueryString["Status"].ToString());
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