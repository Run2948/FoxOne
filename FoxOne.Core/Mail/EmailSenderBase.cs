using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
namespace FoxOne.Core
{
    public abstract class EmailSenderBase : IEmailSender
    {
        public SmtpEmailSenderConfiguration _configuration;

        protected EmailSenderBase()
        {
            _configuration = new SmtpEmailSenderConfiguration();
        }


        public void Send(string to, string subject, string body, bool isBodyHtml = true)
        {
            Send(_configuration.DefaultFromAddress, to, subject, body, isBodyHtml);
        }

        public void Send(string from, string to, string subject, string body, bool isBodyHtml = true)
        {
            string[] toSplit = to.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            var mailMessage = new MailMessage()
            {
                From = new MailAddress(from),
                IsBodyHtml = isBodyHtml,
                Subject = subject,
                Body = body
            };
            foreach(var t in toSplit)
            {
                mailMessage.To.Add(t);
            }
            Send(mailMessage);
        }


        public void Send(MailMessage mail, bool normalize = true)
        {
            if (normalize)
            {
                NormalizeMail(mail);
            }

            SendEmail(mail);
        }

        protected abstract void SendEmail(MailMessage mail);

        protected virtual void NormalizeMail(MailMessage mail)
        {
            if (mail.From == null || mail.From.Address.IsNullOrEmpty())
            {
                mail.From = new MailAddress(
                    _configuration.DefaultFromAddress,
                    _configuration.DefaultFromDisplayName,
                    Encoding.UTF8
                    );
            }

            if (mail.HeadersEncoding == null)
            {
                mail.HeadersEncoding = Encoding.UTF8;
            }

            if (mail.SubjectEncoding == null)
            {
                mail.SubjectEncoding = Encoding.UTF8;
            }

            if (mail.BodyEncoding == null)
            {
                mail.BodyEncoding = Encoding.UTF8;
            }
        }
    }
}
