using System.Collections.Generic;

namespace WebApplication5.Models
{
    public class Result
    {
        public int Id { get; set; }
        public List<UserAnswer> UserAnswers { get; set; }
        public Result()
        {
            UserAnswers = new List<UserAnswer>();
        }
    }

}