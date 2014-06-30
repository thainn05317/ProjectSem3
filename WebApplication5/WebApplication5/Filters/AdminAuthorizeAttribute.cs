using System;
using System.Web.Mvc;
using System.Web.Routing;
using WebApplication5.Models;
using WebApplication5.Helpers;
namespace WebApplication5.Filters
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        private const string ERROR_URL = "http://localhost:1239/Admin/Error?id=";
        private const string LOGIN_URL = "http://localhost:1239/Admin/Login?returnUrl=";
        private OnlineAptitudeTestEntities db = new OnlineAptitudeTestEntities();
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["UserInfo"] == null)
            {
                if (filterContext.HttpContext.Request.Cookies["UserName"] != null && filterContext.HttpContext.Request.Cookies["Password"] != null)
                {
                    var manager = db.Managers.Find(filterContext.HttpContext.Request.Cookies["UserName"].Value);
                    if (manager != null && manager.Pass.Equals(filterContext.HttpContext.Request.Cookies["Password"].Value))
                    {
                        filterContext.HttpContext.Session["UserInfo"] = manager;
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult(LOGIN_URL + filterContext.HttpContext.Request.RawUrl);
                    }
                }
                else
                {
                    filterContext.Result = new RedirectResult(LOGIN_URL + filterContext.HttpContext.Request.RawUrl);
                }
            }
            else
            {
                if (!(filterContext.HttpContext.Session["UserInfo"] is Manager))
                {
                    filterContext.Result = new RedirectResult(ERROR_URL + 1);
                }
            }
        }
    }
}