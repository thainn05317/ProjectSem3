using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication5.Models
{
    public class ResultViewModel
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public int Mark { get; set; }
        public int CorrectAnswer { get; set; }
        public int GKMark { get; set; }
        public int GKCorrect { get; set; }
        public int MAMark { get; set; }
        public int MACorrect { get; set; }
        public int CTMark { get; set; }
        public int CTCorrect { get; set; }
        public string DateTaken { get; set; }
    }
}