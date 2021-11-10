using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DocumentSender.Services
{
    public class EmailService
    {
       //
       public void SendMessage(string to, string bcc, string Subject,string body, MemoryStream pdfStream)
        {
            MailMessage message = new MailMessage();
            //message.From = new MailAddress("enockbwana@gmail.com");
            message.From = new MailAddress("results@checkupsmed.com");
            message.To.Add(new MailAddress(to));
            message.Bcc.Add(new MailAddress(bcc));
            message.Subject = Subject;
            message.Body = body;
            message.Attachments.Add(new Attachment(pdfStream, "Document.pdf"));
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("results@checkupsmed.com", "checkups@123"),
                EnableSsl = true,
            };
            // send email
            smtpClient.Send(message);

        }
        public void SendMessage2(string to, string cc, string bcc, string Subject, string body, MemoryStream pdfStream)
        {
            MailMessage message = new MailMessage();
            //message.From = new MailAddress("enockbwana@gmail.com");
            message.From = new MailAddress("results@checkupsmed.com");
            message.To.Add(new MailAddress(to));
            message.CC.Add(new MailAddress(cc));
            message.Bcc.Add(new MailAddress(bcc));
            message.Subject = Subject;
            message.Body = body;
            message.IsBodyHtml = true;
            message.Attachments.Add(new Attachment(pdfStream, "Document.pdf"));

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("results@checkupsmed.com", "checkups@123"),
                EnableSsl = true,
            };
            // send email
            smtpClient.Send(message);

        }


    }
}
