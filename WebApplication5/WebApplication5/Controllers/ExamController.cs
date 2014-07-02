using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplication5.Filters;
using WebApplication5.Helpers;
using WebApplication5.Models;
namespace WebApplication5.Controllers
{
    public class ExamController : Controller
    {
        private OnlineAptitudeTestEntities db = new OnlineAptitudeTestEntities();
        [CustomActionFilter("")]
        // GET: Exam
        public ActionResult Index()
        {
            return View();
        }
        [CustomActionFilter("GK")]
        [NoCache]
        public async Task<ActionResult> GeneralKnowledge()
        {
            Candidate candidate = (Candidate)Session["UserInfo"];
            candidate.DateTaken = DateTime.Now.Date;
            db.Entry(candidate).State = EntityState.Modified;
            await db.SaveChangesAsync();
            Session["UserInfo"] = candidate;
            return View(GetQuestions("GK"));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GeneralKnowledge(IList<Result> results)
        {
            GetResults(results, "GK");
            return RedirectToAction("Mathematics");
        }
        [CustomActionFilter("MA")]
        [NoCache]
        public ActionResult Mathematics()
        {
            return View(GetQuestions("MA"));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Mathematics(IList<Result> results)
        {
            GetResults(results, "MA");
            return RedirectToAction("ComputerTechnology");
        }
        [CustomActionFilter("CT")]
        [NoCache]
        public ActionResult ComputerTechnology()
        {
            return View(GetQuestions("CT"));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ComputerTechnology(IList<Result> results)
        {
            GetResults(results, "CT");
            Candidate candidate = (Candidate)Session["UserInfo"];
            candidate.Result = (int)Session["Mark"];
            candidate.Passed = candidate.Result > 10 ? true : false;
            db.Entry(candidate).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Results");
        }
        public async Task<ActionResult> Results()
        {
            Candidate candidate = (Candidate)Session["UserInfo"];
            if (candidate.Passed)
            {
                EmailSender.SendEmail(candidate.Email, "Congratulations from Webster organization", "You have passed the online test, further contact will be made, please wait patiently");
            }
            ResultViewModel rvm = new ResultViewModel();
            rvm.CorrectAnswer = (int)Session["TotalCorrect"];
            rvm.Mark = candidate.Result.Value;
            rvm.Name = candidate.Name;
            rvm.Id = candidate.Id;
            rvm.MAMark = (int)Session["MAMark"];
            rvm.MACorrect = (int)Session["MACorrect"];
            rvm.GKMark = (int)Session["GKMark"];
            rvm.GKCorrect = (int)Session["GKCorrect"];
            rvm.CTMark = (int)Session["CTMark"];
            rvm.CTCorrect = (int)Session["CTCorrect"];
            rvm.DateTaken = DateTime.Now.ToString();
            candidate.Result = rvm.Mark;
            db.Entry(candidate).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return View(rvm);
        }
        public List<Question> GetQuestions(string session)
        {
            List<Question> list;
            if (Session[session] == null)
            {
                int categoryId = 0;
                switch (session)
                {
                    case "GK":
                        categoryId = 1;
                        break;
                    case "MA":
                        categoryId = 2;
                        break;
                    case "CT":
                        categoryId = 3;
                        break;
                    default:
                        break;
                }
                list = new List<Question>();
                for (int i = 1; i < 6; i++)
                {
                    list.Add(db.GetQuestion(categoryId, i).First());
                }
                Random rd = new Random();
                for (int i = 0; i < list.Count; i++)
                {
                    int n = rd.Next(list.Count);
                    Question temp = list[i];
                    list[i] = list[n];
                    list[n] = temp;
                }
                Session[session] = list;
            }
            else
            {
                list = null;
            }
            return list;
        }
        public void GetResults(IList<Result> results, string session)
        {
            List<Question> questions = (List<Question>)Session[session];
            int mark = 0;
            int totalCorrect = 0;
            foreach (var question in questions)
            {
                Result r = null;
                foreach (var item in results)
                {
                    if (item.Id == question.Id)
                    {
                        r = item;
                        break;
                    }
                }
                List<Answer> correctAnswers = new List<Answer>();
                foreach (var correct in question.Answers)
                {
                    if (correct.IsRightAnswer)
                    {
                        correctAnswers.Add(correct);
                    }
                }
                List<UserAnswer> userAnswers = new List<UserAnswer>();
                foreach (var item in r.UserAnswers)
                {
                    if (item.Checked)
                    {
                        userAnswers.Add(item);
                    }
                }
                if (userAnswers.Count != 0)
                {
                    int count = 0;
                    foreach (var item in correctAnswers)
                    {
                        foreach (var ua in userAnswers)
                        {
                            if (ua.AnswerId == item.Id)
                            {
                                count++;
                            }
                        }
                    }
                    if (count == userAnswers.Count && count == correctAnswers.Count)
                    {
                        mark += question.Mark;
                        totalCorrect++;
                    }
                }
            }
            Session["Mark"] = (Session["Mark"] == null ? 0 : (int)Session["Mark"]) + mark;
            Session["TotalCorrect"] = (Session["TotalCorrect"] == null ? 0 : (int)Session["TotalCorrect"]) + totalCorrect;
            switch (session)
            {
                case "GK":
                    Session["GKMark"] = (int)Session["Mark"];
                    Session["GKCorrect"] = (int)Session["TotalCorrect"];
                    break;
                case "MA":
                    Session["MAMark"] = (int)Session["Mark"] - (int)Session["GKMark"];
                    Session["MACorrect"] = (int)Session["TotalCorrect"] - (int)Session["GKCorrect"];
                    break;
                case "CT":
                    Session["CTMark"] = (int)Session["Mark"] - (int)Session["MAMark"] - (int)Session["GKMark"];
                    Session["CTCorrect"] = (int)Session["TotalCorrect"] - (int)Session["MACorrect"] - (int)Session["GKCorrect"];
                    break;
                default:
                    break;
            }
        }
    }
}