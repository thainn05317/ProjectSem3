using PagedList;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplication5.Filters;
using WebApplication5.Helpers;
using WebApplication5.Models;
namespace WebApplication5.Controllers
{
    [AdminAuthorizeAttribute]
    public class CandidatesController : Controller
    {
        private OnlineAptitudeTestEntities db = new OnlineAptitudeTestEntities();

        // GET: Candidates
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.AddressSortParm = sortOrder == "Address" ? "address_desc" : "Address";
            ViewBag.GenderSortParm = sortOrder == "Gender" ? "gender_desc" : "Gender";
            ViewBag.BirthdaySortParm = sortOrder == "Birthday" ? "birthday_desc" : "Birthday";
            ViewBag.PassedSortParm = sortOrder == "Passed" ? "passed_desc" : "Passed";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "email_desc" : "Email";
            ViewBag.DateTakenSortParm = sortOrder == "DateTaken" ? "datetaken_desc" : "DateTaken";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var candidates = from q in db.Candidates select q;
            if (!String.IsNullOrEmpty(searchString))
            {
                candidates = candidates.Where(c => c.Name.ToUpper().Contains(searchString.ToUpper()) || c.Email.ToUpper().Contains(searchString.ToUpper()) || c.HomeAddress.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    candidates = candidates.OrderByDescending(q => q.Name);
                    break;
                case "Address":
                    candidates = candidates.OrderBy(q => q.HomeAddress);
                    break;
                case "address_desc":
                    candidates = candidates.OrderByDescending(q => q.HomeAddress);
                    break;
                case "Gender":
                    candidates = candidates.OrderBy(q => q.Gender);
                    break;
                case "gender_desc":
                    candidates = candidates.OrderByDescending(q => q.Gender);
                    break;
                case "Birthday":
                    candidates = candidates.OrderBy(c => c.Birthday);
                    break;
                case "birthday_desc":
                    candidates = candidates.OrderByDescending(c => c.Birthday);
                    break;
                case "Passed":
                    candidates = candidates.OrderBy(c => c.Passed);
                    break;
                case "passed_desc":
                    candidates = candidates.OrderByDescending(c => c.Passed);
                    break;
                case "Email":
                    candidates = candidates.OrderBy(c => c.Email);
                    break;
                case "email_desc":
                    candidates = candidates.OrderByDescending(c => c.Email);
                    break;
                case "DateTaken":
                    candidates = candidates.OrderBy(c => c.DateTaken);
                    break;
                case "datetaken_desc":
                    candidates = candidates.OrderByDescending(c => c.DateTaken);
                    break;
                default:
                    candidates = candidates.OrderBy(q => q.Name);
                    break;
            }
            var items = candidates.ToPagedList(page ?? 1, 10);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_CandidatePartial", items);
            }
            return View(items);
        }

        // GET: Candidates/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidate candidate = await db.Candidates.FindAsync(id);
            if (candidate == null)
            {
                return HttpNotFound();
            }
            return View(candidate);
        }

        // GET: Candidates/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Candidates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Candidate candidate)
        {
            if (ModelState.IsValid)
            {
                candidate.Id = "CAN" + new Random().Next(1000000).ToString();
                candidate.Pass = RandomPassword.Generate(8, 12);
                db.Candidates.Add(candidate);
                await db.SaveChangesAsync();
                EmailSender.SendEmail(candidate.Email, "Your user name and password from Webster organization", "Your user name is :" + candidate.Id + "<br />" + "Your password is:" + candidate.Pass + "<br />" + "Use it to log in and take your aptitude test<br />Please take the test on " + candidate.DateAssigned.ToShortDateString());
                return RedirectToAction("Index");
            }
            return View(candidate);
        }

        // GET: Candidates/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidate candidate = await db.Candidates.FindAsync(id);
            if (candidate == null)
            {
                return HttpNotFound();
            }
            return View(candidate);
        }

        // POST: Candidates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Candidate candidate = await db.Candidates.FindAsync(id);
            db.Candidates.Remove(candidate);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public JsonResult IsEmailAvailable(string Email)
        {
            var isAvail = (from candidate in db.Candidates where candidate.Email == Email select candidate).FirstOrDefault();
            if (isAvail == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult FilterCandidatesByDate(DateTime? DateTaken)
        {
            ViewBag.Date = DateTaken.Value.ToShortDateString();
            var model = db.Candidates.Where(can => can.DateTaken.HasValue && DateTaken.HasValue && can.DateTaken.Value.Day == DateTaken.Value.Day && can.DateTaken.Value.Month == DateTaken.Value.Month && can.DateTaken.Value.Year == DateTaken.Value.Year);
            return View(model);
        }
    }
}
