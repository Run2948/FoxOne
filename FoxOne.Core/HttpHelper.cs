using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
namespace FoxOne.Core
{
    public static class HttpHelper
    {
        public static void GetImage(string imgUrl, string imgPath)
        {
            var url = HttpContext.Current.Request.Url;
            WebClient client = new WebClient();
            FileInfo dirInfo = new FileInfo(imgPath);
            string acceptExts = ".js|.css|.jpg|.png|.bmp|.jpeg|.html|.ico|.gif";
            if (acceptExts.Split('|').Contains(dirInfo.Extension))
            {
                if (!dirInfo.Directory.Exists)
                {
                    dirInfo.Directory.Create();
                }
                try
                {
                    client.DownloadFile(imgUrl, imgPath);
                }
                catch
                {
                }
            }
        }

        public static string GetContent(string url, IDictionary<string, object> parameter = null)
        {
            try
            {
                url = BuildUrl(url, parameter);
                var MyWebClient = new WebClient();
                MyWebClient.Credentials = CredentialCache.DefaultCredentials; //获取或设置用于向Internet资源的请求进行身份验证的网络凭据 
                Byte[] pageData = MyWebClient.DownloadData(url); //从指定网站下载数据 
                //string pageHtml = System.Text.Encoding.Default.GetString(pageData);//如果获取网站页面采用的是GB2312，则使用这句 
                string pageHtml = Encoding.UTF8.GetString(pageData); //如果获取网站页面采用的是UTF-8，则使用这句 
                return pageHtml;
            }
            catch (WebException webEx)
            {
                Console.WriteLine(webEx.Message.ToString());
                return null;
            }
        }

        public static string BuildUrl(string url, HttpRequestBase request)
        {
            if (request == null || string.IsNullOrEmpty(url))
            {
                return url;
            }
            foreach (var p in request.QueryString.AllKeys)
            {
                if (url.IndexOf('?') > 0)
                {
                    url += string.Format("&{0}={1}", p, request.QueryString[p]);
                }
                else
                {
                    url += string.Format("?{0}={1}", p, request.QueryString[p]);
                }
            }
            return url;
        }

        public static string BuildUrl(string url, IDictionary<string, object> parameters)
        {
            if (parameters.IsNullOrEmpty() || string.IsNullOrEmpty(url))
            {
                return url;
            }
            foreach (var p in parameters)
            {
                if (url.IndexOf('?') > 0)
                {
                    url += string.Format("&{0}={1}", p.Key, p.Value);
                }
                else
                {
                    url += string.Format("?{0}={1}", p.Key, p.Value);
                }
            }
            return url;
        }
    }
}
