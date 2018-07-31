using System.Net;
using System.Net.Mail;

namespace Library.Classes
{
    public class EmailClass
    {
        public void SendMessage(string email, string bookName)
        {
            MailAddress from = new MailAddress("bazhenovlibrary@gmail.com", "Library");
            MailAddress to = new MailAddress(email);
            MailMessage message = new MailMessage(from, to);
            message.Subject = "You took the book";
            message.Body = "You took the following books in our library: " + bookName;
            message.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.Credentials = new NetworkCredential("bazhenovlibrary@gmail.com", "q9379992");
            smtp.EnableSsl = true;
            smtp.Send(message);
        }
    }
}