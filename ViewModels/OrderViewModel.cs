using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    //int productId, bool desinged_check=false, string fronttext, string backtext, string description, string phonenumber, int count, HttpPostedFileBase[] files
    public class SendOrderViewModel
    {
        public bool isDesign { get; set; }
        public string frontText { get; set; }
        public string backText { get; set; }
        public string description { get; set; }
        public string phonenumber  { get; set; }
        public int count { get; set; }
        public HttpPostedFileBase[] files { get; set; }
    }
}
