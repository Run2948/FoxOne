

namespace FoxOne.Core
{
    public class SmtpEmailSenderConfiguration 
    {
        public string Host
        {
            get { return SysConfig.AppSettings["MailHost"]; }
        }

        public int Port
        {
            get { return SysConfig.AppSettings["MailPort"].ConvertTo<int>(); }
        }

        public string UserName
        {
            get { return SysConfig.AppSettings["MailUserName"]; }
        }

        public string Password
        {
            get { return SysConfig.AppSettings["MailPassword"]; }
        }

        public string Domain
        {
            get { return ""; }
        }

        public bool EnableSsl
        {
            get { return true; }
        }

        public bool UseDefaultCredentials
        {
            get { return false; }
        }

        public string DefaultFromAddress
        {
            get { return this.UserName; }
        }

        public string DefaultFromDisplayName
        {
            get { return "0124"; }
        }
    }
}