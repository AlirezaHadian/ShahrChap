using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class VisitSiteViewModel
    {
        public int VisitToday { get; set; }
        public int VisitYesterday { get; set; }
        public int VisitMonth { get; set; }
        public int VisitPrevMonth { get; set; }
        public int TotalVisit { get; set; }
        public int Online { get; set; }
    }
}
