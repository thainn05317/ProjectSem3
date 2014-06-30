using System;
using System.Web.Mvc;
using System.Web.Routing;
using WebApplication5.Models;
namespace WebApplication5.Filters
{
    public class CustomActionFilter : AuthorizeAttribute
    {
        private const string LOGIN_URL = "http://localhost:1239/HomePage/Login?returnUrl=";
        private const string ERROR_URL = "http://localhost:1239/HomePage/Error?msg=";
        public string Step { get; set; }
        public CustomActionFilter(string Step)
        {
            this.Step = Step;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["UserInfo"] == null)
            {
                filterContext.Result = new RedirectResult(LOGIN_URL + filterContext.HttpContext.Request.RawUrl);
            }
            else
            {
                Candidate candidate = (Candidate)filterContext.HttpContext.Session["UserInfo"];
                if (candidate.Result == null && candidate.DateAssigned.Day == DateTime.Now.Day && candidate.DateAssigned.Month == DateTime.Now.Month && candidate.DateAssigned.Year == DateTime.Now.Year)
                {
                    if (Step.Equals("GK"))
                    {
                        if (filterContext.HttpContext.Session["MA"] == null && filterContext.HttpContext.Session["GK"] == null && filterContext.HttpContext.Session["CT"] == null)
                        {
                        }
                        else
                        {
                            filterContext.Result = new RedirectResult(ERROR_URL + "1");
                        }
                    }
                    else if (Step.Equals("MA"))
                    {
                        if (filterContext.HttpContext.Session["GK"] != null && filterContext.HttpContext.Session["MA"] == null && filterContext.HttpContext.Session["CT"] == null)
                        {
                        }
                        else
                        {
                            filterContext.Result = new RedirectResult(ERROR_URL + "1");
                        }
                    }
                    else if (Step.Equals("CT"))
                    {
                        if (filterContext.HttpContext.Session["GK"] != null && filterContext.HttpContext.Session["MA"] != null && filterContext.HttpContext.Session["CT"] == null)
                        {
                        }
                        else
                        {
                            filterContext.Result = new RedirectResult(ERROR_URL + "1");
                        }
                    }
                    else
                    {
                    }
                }
                else
                {
                    if (candidate.Result != null)
                    {
                        filterContext.Result = new RedirectResult(ERROR_URL + "2");
                    }
                    else
                    {
                        if (!(candidate.DateAssigned.Day == DateTime.Now.Day && candidate.DateAssigned.Month == DateTime.Now.Month && candidate.DateAssigned.Year == DateTime.Now.Year))
                        {
                            filterContext.Result = new RedirectResult(ERROR_URL + "3");
                        }
                    }
                }
            }
        }
    }
}