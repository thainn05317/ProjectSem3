using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using WebApplication5.Filters;
using WebApplication5.Helpers;
using WebApplication5.Models;
namespace WebApplication5.Controllers
{
    [AdminAuthorizeAttribute]
    public class StatisticsController : Controller
    {

        private OnlineAptitudeTestEntities db = new OnlineAptitudeTestEntities();
        // GET: Statistics
        public ActionResult Index()
        {
            ViewBag.Factor = "Day";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(StatisticsViewModel svm)
        {
            ViewBag.FromDate = svm.FromDate.ToShortDateString();
            ViewBag.ToDate = svm.ToDate.ToShortDateString();
            ViewBag.Factor = svm.Factor;
            List<ChartViewModel> list = new List<ChartViewModel>();
            switch (svm.Factor)
            {
                case "Day":
                    for (int i = 0; svm.ToDate.CompareTo(svm.FromDate.AddDays(i)) >= 0; i++)
                    {
                        ChartViewModel cvm = new ChartViewModel();
                        DateTime temp = svm.FromDate.AddDays(i);
                        int year = temp.Year;
                        string month = temp.Month < 10 ? "0" + temp.Month.ToString() : temp.Month.ToString();
                        string day = temp.Day < 10 ? "0" + temp.Day.ToString() : temp.Day.ToString();
                        cvm.Time = "" + year + "-" + month + "-" + day;
                        DateTime newDate = svm.FromDate.AddDays(i);
                        var total = from can in db.Candidates where can.DateTaken.HasValue && can.DateTaken.Value.Day == newDate.Day && can.DateTaken.Value.Month == newDate.Month && can.DateTaken.Value.Year == newDate.Year select can;
                        var passed = total.Where(can => can.Passed);
                        cvm.Passed = passed.ToList().Count();
                        cvm.Total = total.ToList().Count();
                        list.Add(cvm);
                    }
                    break;
                case "Week":
                    Calendar calendar = CultureInfo.InvariantCulture.Calendar;
                    for (int i = 0; calendar.GetWeekOfYear(svm.FromDate.AddDays(i), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) <= calendar.GetWeekOfYear(svm.ToDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday); i += 7)
                    {
                        ChartViewModel cvm = new ChartViewModel();
                        DateTime startDate = FirstDayOfWeekUtility.GetFirstDayOfWeek(svm.FromDate.AddDays(i));
                        int total = 0;
                        int passed = 0;
                        for (int n = 0; n < 6; n++)
                        {
                            DateTime temp = startDate.AddDays(n);
                            var totalCandidates = (from can in db.Candidates where can.DateTaken.HasValue && can.DateTaken.Value.Day == temp.Day && can.DateTaken.Value.Month == temp.Month && can.DateTaken.Value.Year == temp.Year select can).ToList();
                            var passedCandidates = totalCandidates.Where(can => can.Passed);
                            total += totalCandidates.Count;
                            passed += passedCandidates.ToList().Count;
                        }
                        cvm.Passed = passed;
                        cvm.Total = total;
                        cvm.Time = startDate.ToShortDateString() + "-" + startDate.AddDays(6).ToShortDateString();
                        list.Add(cvm);
                    }
                    break;
                case "Month":
                    for (int i = 0; svm.FromDate.AddMonths(i).Month <= svm.ToDate.Month; i++)
                    {
                        ChartViewModel cvm = new ChartViewModel();
                        int total = 0;
                        int passed = 0;
                        DateTime newFromDate = svm.FromDate.AddMonths(i);
                        for (int n = 0; newFromDate.AddDays(n).Month == newFromDate.Month; n++)
                        {
                            DateTime temp = newFromDate.AddDays(n);
                            var totalCandidates = from can in db.Candidates where can.DateTaken.HasValue && can.DateTaken.Value.Day == temp.Day && can.DateTaken.Value.Month == temp.Month && can.DateTaken.Value.Year == temp.Year select can;
                            var passedCandidates = totalCandidates.Where(can => can.Passed);
                            total += totalCandidates.ToList().Count;
                            passed += passedCandidates.ToList().Count;
                        }
                        cvm.Passed = passed;
                        cvm.Total = total;
                        cvm.Time = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(newFromDate.Month);
                        list.Add(cvm);
                    }
                    break;
                default:
                    break;
            }
            return View(list);
        }
    }
}