using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplication5.Models;
namespace WebApplication5.Controllers
{
    public class HomePageController : Controller
    {
        private OnlineAptitudeTestEntities db = new OnlineAptitudeTestEntities();
        // GET: HomePage
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Recruitment()
        {
            return View();
        }
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
                var candidate = await db.Candidates.FindAsync(lvm.UserName);
                if (candidate != null && candidate.Pass.Equals(lvm.Password))
                {
                    Session["UserInfo"] = candidate;
                    if (String.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction("Index", "HomePage");
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                }
            }
            ViewBag.ErrorMsg = "Your user name and password are incorrect";
            ViewBag.ReturnUrl = returnUrl;
            return View(lvm);
        }
        public ActionResult Error(int? msg)
        {
            string message = "";
            if (msg == null)
            {
                message = "The resoureces you are trying to access is limited, or you do not have to right to access. Contact the administrator for further information";
            }
            else
            {
                int id = msg.Value;
                switch (id)
                {
                    case 1:
                        message = "You did not follow the instruction, your exam ends here";
                        break;
                    case 2:
                        message = "You have taken the test before, the resources you are trying to accesss is limitted";
                        break;
                    case 3:
                        message = "Your test is not available for you now, should you read carefully the email we had sent to you before to check the time";
                        break;
                    default:
                        break;
                }
            }
            ViewBag.Msg = message;
            return View();
        }
        public ActionResult SignOut(string returnUrl)
        {
            Session["UserInfo"] = null;
            return Redirect(returnUrl);
        }
        public ActionResult AboutUs()
        {
            return View();
        }
        public ActionResult ContactUs()
        {
            return View();
        }
    }
}