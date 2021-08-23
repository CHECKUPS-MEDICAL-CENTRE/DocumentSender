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
       public void SendMessage(string to,string Subject,string body, MemoryStream pdfStream)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress("enockbwana@gmail.com");
            message.To.Add(new MailAddress(to));
            message.Subject = Subject;
            message.Body = body;
            message.Attachments.Add(new Attachment(pdfStream, "Document.pdf"));

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("enockbwana@gmail.com", "basigwa95"),
                EnableSsl = true,
            };
            // send email
            smtpClient.Send(message);

        }

    }
}
