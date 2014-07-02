using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication5.Models
{
    public class ChartViewModel
    {
        public string Time { get; set; }
        public int Passed { get; set; }
        public int Total { get; set; }
    }
}