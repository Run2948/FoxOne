using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
namespace FoxOne.Core
{

    public class SmtpEmailSender : EmailSenderBase
    {
        public SmtpEmailSender()
        {
        }

        public SmtpClient BuildClient()
        {
            var host = _configuration.Host;
            var port = _configuration.Port;

            var smtpClient = new SmtpClient(host, port);
            try
            {
                if (_configuration.EnableSsl)
                {
                    smtpClient.EnableSsl = true;
                }

                if (_configuration.UseDefaultCredentials)
                {
                    smtpClient.UseDefaultCredentials = true;
                }
                else
                {
                    smtpClient.UseDefaultCredentials = false;

                    var userName = _configuration.UserName;
                    if (!userName.IsNullOrEmpty())
                    {
                        var password = _configuration.Password;
                        var domain = _configuration.Domain;
                        smtpClient.Credentials = !domain.IsNullOrEmpty()
                            ? new NetworkCredential(userName, password, domain)
                            : new NetworkCredential(userName, password);
                    }
                }

                return smtpClient;
            }
            catch
            {
                smtpClient.Dispose();
                throw;
            }
        }

        protected override void SendEmail(MailMessage mail)
        {
            using (var smtpClient = BuildClient())
            {
                smtpClient.Send(mail);
            }
        }
    }
}