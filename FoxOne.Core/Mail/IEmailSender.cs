using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace FoxOne.Core
{
    public interface IEmailSender
    {

        /// <summary>
        /// 发送邮件
        /// </summary>
        void Send(string to, string subject, string body, bool isBodyHtml = true);

        /// <summary>
        /// 发送邮件
        /// </summary>
        void Send(string from, string to, string subject, string body, bool isBodyHtml = true);

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mail">要发送的邮件实例</param>
        /// <param name="normalize">
        /// 是否发送普通格式邮件
        /// 如果为false，则在设置之前先将邮件进行UTF-8编码
        /// </param>
        void Send(MailMessage mail, bool normalize = true);

    }
}
