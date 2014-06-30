using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
namespace WebApplication5.Helpers
{
    public static class EmailSender
    {
        public static void SendEmail(string to, string subject, string body)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(to);
            mail.From = new MailAddress("badboy0610@gmail.com");
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("badboy0610@gmail.com", "Lmh123456");
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
    }
}