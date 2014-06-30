using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication5
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new CustomRazorViewEngine());
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
    public class CustomRazorViewEngine : RazorViewEngine
    {
        public CustomRazorViewEngine()
        {
            base.AreaViewLocationFormats = new string[] {
                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml"
            };

            base.AreaMasterLocationFormats = new string[] {
                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml"
            };

            base.AreaPartialViewLocationFormats = new string[] {
                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml"
            };

            base.ViewLocationFormats = new string[] {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml"
            };

            base.PartialViewLocationFormats = new string[] {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml"
            };

            base.MasterLocationFormats = new string[] {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml"
            };
        }
    }
}
