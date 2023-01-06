using DataLayer;
using DataLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ShahrChap.Controllers
{
    public class ShopController : ApiController
    {
        private UnitOfWork db = new UnitOfWork();
        // GET: api/Shop
        public int Get()
        {
            if (!User.Identity.IsAuthenticated)
            {
                List<ViewModels.ShopCartItem> list = new List<ViewModels.ShopCartItem>();
                var sessions = HttpContext.Current.Session;
                if (sessions["ShopCart"] != null)
                {
                    list = sessions["ShopCart"] as List<ViewModels.ShopCartItem>;
                }
                return list.Sum(l => l.Count);
            }
            else
            {
                int userId = db.UserRepository.Get().Single(u => u.UserName == User.Identity.Name).UserID;
                List<ShopCart> shopCart = db.ShopCartRepository.Get().Where(s => s.UserID == userId).ToList();
                return shopCart.Sum(f => f.Count);
            }
        }

        // GET: api/Shop/5
        public int Get(int id, int count)
        {
            var product = db.ProductsRepository.GetById(id);
            if (User.Identity.IsAuthenticated)
            {
                int userId = db.UserRepository.Get().Single(u => u.UserName == User.Identity.Name).UserID;
                List<ShopCart> shopList = new List<ShopCart>();
                var shop = db.ShopCartRepository.Get(s => s.UserID == userId);
                if (shop != null)
                {
                    shopList = shop.ToList();
                }
                if (shopList.Any(l => l.ProductID == id))
                {
                    int index = shopList.FindIndex(p => p.ProductID == id);
                    shopList[index].Count += count;
                    int shopId = shopList[index].SC_ID;
                    var shopCart = db.ShopCartRepository.GetById(shopId);

                    shopCart.Count = shopList[index].Count;
                    shopCart.Sum = product.Price * shopList[index].Count;
                    db.ShopCartRepository.Update(shopCart);
                    db.Save();
                }
                else
                {
                    shopList.Add(new ShopCart()
                    {
                        ProductID = id,
                        Count = count,
                        Price = product.Price,
                        UserID = userId,
                        Sum = product.Price * count
                    });
                    ShopCart shopCart = new ShopCart()
                    {
                        Count = count,
                        ProductID = id,
                        Price = product.Price,
                        Sum = product.Price * count,
                        UserID = userId,
                    };
                    db.ShopCartRepository.Insert(shopCart);
                    db.Save();
                }
            }
            else
            {
                List<ViewModels.ShopCartItem> list = new List<ViewModels.ShopCartItem>();
                var sessions = HttpContext.Current.Session;
                if (sessions["ShopCart"] != null)
                {
                    list = sessions["ShopCart"] as List<ViewModels.ShopCartItem>;
                }
                if (list.Any(p => p.ProductID == id))
                {
                    int index = list.FindIndex(p => p.ProductID == id);
                    list[index].Count += count;
                }

                else
                {
                    list.Add(new ViewModels.ShopCartItem()
                    {
                        ProductID = id,
                        Count = count
                    });
                }
                sessions["ShopCart"] = list;
            }
            return Get();
        }
    }
}
