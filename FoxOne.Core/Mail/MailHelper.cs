using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxOne.Core
{
    public class MailHelper
    {
        public static void SendMail(string to, string subject, string body)
        {
            Task.Factory.StartNew(() =>
            {
                ObjectHelper.GetObject<IEmailSender>().Send(to, subject, body);
            });
        }
    }
}
