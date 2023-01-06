using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using System.Threading;
using Utilities;
using DataLayer.Context;

namespace ShahrChap
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);   
            Application["Online"] = 0;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var persianCulture = new PersianCulture();
            Thread.CurrentThread.CurrentCulture = persianCulture;
            Thread.CurrentThread.CurrentUICulture = persianCulture;
        }

        protected void Application_PostAuthorizeRequest()
        {
            System.Web.HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        }

        protected void Session_Start()
        {

            int online = int.Parse(Application["Online"].ToString());
            online += 1;
            Application["Online"] = online;

            DateTime dtNow = DateTime.Now.Date;
            string ip = Request.UserHostAddress;

            using(UnitOfWork db=new UnitOfWork())
            {
                if(!db.SiteVisitRepository.Get().Any(s=> s.IP==ip && s.Date == dtNow))
                {
                    db.SiteVisitRepository.Insert(new DataLayer.SiteVisit()
                    {
                        IP = ip,
                        Date = DateTime.Now
                    });
                    db.Save();
                }
            }
        }

        protected void Session_End()
        {
            int online = int.Parse(Application["Online"].ToString());
            online -= 1;
            Application["Online"] = online;
        }
    }
}