using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;

namespace ViewModels
{
    public class ShowProductFeaturesViewModel
    {
        public string FeatureTitle { get; set; }
        public List<string> Values { get; set; }
    }
    public class AllProductsViewModel
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public int Price { get; set; }
        public string ImageName { get; set; }
        public bool IsTwoFace { get; set; }
        public int Count { get; set; }
        public bool IsExist { get; set; }
    }
}
