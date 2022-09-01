using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShahrChap.Controllers
{
    public class ManageEmailsController : Controller
    {
        // GET: ManageEmail
        public ActionResult ActivationEmail()
        {
            return PartialView();
        }

        public ActionResult RecoveryPassword()
        {
            return PartialView();
        }
        public ActionResult EnableEmail()
        {
            return PartialView();
        }
    }
}