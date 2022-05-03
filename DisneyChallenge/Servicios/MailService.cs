using DisneyChallenge.Helpers;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.IO;
using System.Threading.Tasks;
namespace DisneyChallenge.Servicios
{
    public class MailService : IMailService
    {        
        private readonly MailSettings _mailSettings;

        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            /*
             * The basic idea is to create an object of MimeMessage ( a class from Mimekit ) and send it using a 
             * SMTPClient instance (Mailkit).
               Creates a new object of MimeMessage and adds in the Sender, To Address and Subject to this object.
               We will be filling the message related data (subject, body) from the mailRequest and the data we 
               get from our JSON File.
             * */
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
           
            
            
            var builder = new BodyBuilder();
            //Here we the HTML part of the email from the Body property of the request.
            builder.HtmlBody = mailRequest.Body;
            //Finally, add HTML Body to the Body of the Email.
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email); //Send the Message using the smpt’s SendMailAsync Method
            smtp.Disconnect(true);
        }
    }
}
