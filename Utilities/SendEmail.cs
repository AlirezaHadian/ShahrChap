using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Utilities
{
    public class SendEmail
    {
        public static void Send(string To,string Subject,string Body)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("store@shahrechapp.ir", "شهر چاپ");
            SmtpClient SmtpServer = new SmtpClient("mail.shahrechapp.ir");
            mail.To.Add(To);
            mail.Subject = Subject;
            mail.Body = Body;
            mail.IsBodyHtml = true;

            //System.Net.Mail.Attachment attachment;
            // attachment = new System.Net.Mail.Attachment("c:/textfile.txt");
            // mail.Attachments.Add(attachment);

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("store@shahrechapp.ir", "Shahrchap8763");
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);

        }
    }
}