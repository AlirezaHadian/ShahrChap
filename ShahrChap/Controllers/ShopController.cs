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
        // GET: api/Shop
        public int Get()
        {
            List<ViewModels.ShopCartItem> list = new List<ViewModels.ShopCartItem>();
            var sessions = HttpContext.Current.Session;
            if (sessions["ShopCart"] != null)
            {
                list = sessions["ShopCart"] as List<ViewModels.ShopCartItem>;
            }

            return list.Sum(l => l.Count);
        }

        // GET: api/Shop/5
        public int Get(int id, int count)
        {                                          
            List<ViewModels.ShopCartItem> list = new List<ViewModels.ShopCartItem>();
            var sessions = HttpContext.Current.Session;
            if (sessions["ShopCart"] != null)
            {
                list = sessions["ShopCart"] as List<ViewModels.ShopCartItem>; 
            }
            if(list.Any(p=> p.ProductID == id))
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
            return Get();
        }
    }
}
