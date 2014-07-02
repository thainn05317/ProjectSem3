using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication5.Models;
using WebApplication5.Helpers;
namespace WebApplication5.Controllers
{
    public class AdminController : Controller
    {
        private OnlineAptitudeTestEntities db = new OnlineAptitudeTestEntities();
        // GET: Admin
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel lvm, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var manager = await db.Managers.FindAsync(lvm.UserName);
                if (manager != null && PasswordHash.ValidatePassword(lvm.Password, manager.Pass))
                {
                    Session["UserInfo"] = manager;
                    if (lvm.RememberMe)
                    {
                        HttpCookie userNameCookie = new HttpCookie("UserName");
                        HttpCookie passwordCookie = new HttpCookie("Password");
                        userNameCookie.Value = lvm.UserName;
                        passwordCookie.Value = manager.Pass;
                        Response.Cookies.Add(userNameCookie);
                        Response.Cookies.Add(passwordCookie);
                    }
                    if (String.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction("Index", "Questions");
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                }
                else
                {
                    ViewBag.ErrorMsg = "Your user name and password are incorrect";
                }
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        public ActionResult Error(int? id)
        {
            ViewBag.ErrorMsg = id == null ? "Unknonwn Error" : "You do not have the right to access this admin page";
            return View();
        }
    }
}