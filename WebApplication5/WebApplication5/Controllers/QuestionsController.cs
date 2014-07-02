using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplication5.Filters;
using WebApplication5.Models;
namespace WebApplication5.Controllers
{
    [AdminAuthorizeAttribute]
    public class QuestionsController : Controller
    {
        private OnlineAptitudeTestEntities db = new OnlineAptitudeTestEntities();

        // GET: Questions
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? Mark, int? CategoryId, string AdvancedSearchString, DateTime? DateCreated)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.MarkSortParm = sortOrder == "Mark" ? "mark_desc" : "Mark";
            ViewBag.CategorySortParm = sortOrder == "Category" ? "category_desc" : "Category";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var questions = from q in db.Questions select q;
            if (!String.IsNullOrEmpty(AdvancedSearchString))
            {
                questions = questions.Where(q => q.Name.ToUpper().Contains(AdvancedSearchString.ToUpper()));
            }
            else
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    questions = questions.Where(q => q.Name.ToUpper().Contains(searchString.ToUpper()) || q.Category.Name.ToUpper().Contains(searchString.ToUpper()));
                }
            }
            if (CategoryId != null)
            {
                questions = questions.Where(q => q.CategoryId == CategoryId);
            }
            if (Mark != null)
            {
                questions = questions.Where(q => q.Mark == Mark);
            }
            if (DateCreated != null)
            {
                DateTime dt = (DateTime)DateCreated;
                questions = questions.Where(q => q.DateCreated.Year == dt.Year && q.DateCreated.Month == dt.Month && q.DateCreated.Day == dt.Day);
            }
            switch (sortOrder)
            {
                case "name_desc":
                    questions = questions.OrderByDescending(q => q.Name);
                    break;
                case "Mark":
                    questions = questions.OrderBy(q => q.Mark);
                    break;
                case "mark_desc":
                    questions = questions.OrderByDescending(q => q.Mark);
                    break;
                case "category_desc":
                    questions = questions.OrderByDescending(q => q.Category.Name);
                    break;
                case "Category":
                    questions = questions.OrderBy(q => q.Category.Name);
                    break;
                default:
                    questions = questions.OrderBy(q => q.Name);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var items = questions.ToPagedList(pageNumber, pageSize);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_QuestionPartial", items);
            }
            return View(items);
        }

        // GET: Questions/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = await db.Questions.FindAsync(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // GET: Questions/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,CategoryId,Mark,Answers,DateCreated")] Question question)
        {
            if (ModelState.IsValid)
            {
                var list = new List<Answer>(question.Answers);
                list.RemoveAll(ans => String.IsNullOrEmpty(ans.Name));
                question.Answers = list;
                db.Questions.Add(question);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", question.CategoryId);
            return View(question);
        }

        // GET: Questions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = await db.Questions.FindAsync(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", question.CategoryId);
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,CategoryId,Mark")] Question question)
        {
            if (ModelState.IsValid)
            {
                db.Entry(question).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", question.CategoryId);
            return View(question);
        }

        // GET: Questions/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = await db.Questions.FindAsync(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Question question = await db.Questions.FindAsync(id);
            foreach (var item in question.Answers)
            {
                db.Answers.Remove(item);
                await db.SaveChangesAsync();
            }
            db.Questions.Remove(question);
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
        public JsonResult AutoComplete(string term)
        {
            var questions = db.Questions.Where(q => q.Name.ToUpper().Contains(term.ToUpper()) || q.Category.Name.ToUpper().Contains(term.ToUpper()) || q.Mark.ToString().Contains(term)).Select(q => q.Name);
            return Json(questions, JsonRequestBehavior.AllowGet);
        }
    }
}
