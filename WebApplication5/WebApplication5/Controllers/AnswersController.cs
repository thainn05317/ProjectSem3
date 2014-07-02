using PagedList;
using System;
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
    public class AnswersController : Controller
    {
        private OnlineAptitudeTestEntities db = new OnlineAptitudeTestEntities();

        // GET: Answers
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page,int? QuestionId)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.CorrectChoiceSortParm = sortOrder == "CorrectChoice" ? "correctchoice_desc" : "CorrectChoice";
            ViewBag.QuestionNameSortParm = sortOrder == "QuestionName" ? "questionname_desc" : "QuestionName";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var answers = from a in db.Answers select a;
            if (!String.IsNullOrEmpty(searchString))
            {
                answers = answers.Where(a => a.Name.ToUpper().Contains(searchString.ToUpper()) || a.Question.Name.ToUpper().Contains(searchString.ToUpper()));
            }
            if (QuestionId != null)
            {
                answers = answers.Where(a => a.QuestionId == QuestionId);
            }
            switch (sortOrder)
            {
                case "name_desc":
                    answers = answers.OrderByDescending(q => q.Name);
                    break;
                case "CorrectChoice":
                    answers = answers.OrderBy(q => q.IsRightAnswer);
                    break;
                case "correctchoice_desc":
                    answers = answers.OrderByDescending(q => q.IsRightAnswer);
                    break;
                case "questionname_desc":
                    answers = answers.OrderBy(q => q.Question.Name);
                    break;
                case "QuestionName":
                    answers = answers.OrderByDescending(q => q.Question.Name);
                    break;
                default:
                    answers = answers.OrderBy(q => q.Name);
                    break;
            }
            var items = answers.ToPagedList(page ?? 1, 10);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_AnswerPartial", items);
            }
            return View(items);
        }

        // GET: Answers/Create
        public ActionResult Create()
        {
            ViewBag.QuestionId = new SelectList(db.Questions, "Id", "Name");
            return View();
        }

        // POST: Answers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,QuestionId,IsRightAnswer")] Answer answer)
        {
            if (ModelState.IsValid)
            {
                db.Answers.Add(answer);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.QuestionId = new SelectList(db.Questions, "Id", "Name", answer.QuestionId);
            return View(answer);
        }

        // GET: Answers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = await db.Answers.FindAsync(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            ViewBag.QuestionId = new SelectList(db.Questions, "Id", "Name", answer.QuestionId);
            return View(answer);
        }

        // POST: Answers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,QuestionId,IsRightAnswer")] Answer answer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(answer).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.QuestionId = new SelectList(db.Questions, "Id", "Name", answer.QuestionId);
            return View(answer);
        }

        // GET: Answers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = await db.Answers.FindAsync(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            return View(answer);
        }

        // POST: Answers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Answer answer = await db.Answers.FindAsync(id);
            db.Answers.Remove(answer);
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
            var answers = db.Answers.Where(a => a.Name.ToUpper().Contains(term.ToUpper()) || a.Question.Name.ToUpper().Contains(term.ToUpper())).Select(a => a.Name);
            return Json(answers, JsonRequestBehavior.AllowGet);
        }
    }
}
