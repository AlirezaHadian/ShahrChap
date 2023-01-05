using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ViewModels
{
    public class ShopCartItem
    {
        public int ProductID { get; set; }
        public int Count { get; set; }
    }
    public class ShowCartViewModel
    {
        public int ProductID { get; set; }
        public string Title { get; set; }
        public string ImageName { get; set; }
        public int Count { get; set; }
        public int Price { get; set; }
        public int Sum { get; set; }
    }

    public class AddressViewModel
    {
        public int AddressID { get; set; }
    }
}
