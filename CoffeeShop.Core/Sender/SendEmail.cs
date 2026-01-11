using System.Net.Mail;

namespace CoffeeShop.Core.Sender
{
    public class SendEmail
    {
        public static void Send(string to, string subject, string body)
        {
            var username = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? "confirmmailtest@gmail.com";
            var password = Environment.GetEnvironmentVariable("SMTP_PASSWORD");

            if (string.IsNullOrEmpty(password))
                throw new InvalidOperationException("SMTP password is not configured");

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress(address: username, displayName: "اقامت یاب");
            mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(userName: username, password: password);
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);
        }
    }
}
