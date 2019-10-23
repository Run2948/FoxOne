using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
namespace FoxOne.Core
{
    public static class UploadHelper
    {
        /// <summary>
        /// 存储文件的基路径，默认为网站根目录
        /// </summary>
        public static string BaseDirectory { get; private set; }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="subDir">子目录</param>
        /// <param name="fileName">保存的文件名，如果要用上传文件的文件名，则传空值</param>
        /// <param name="useDateFolder">是否在子目录中建立以当前日期为名称的子目录来保存文件</param>
        /// <returns></returns>
        public static string Upload(HttpPostedFile file, string subDir, string fileName,bool useDateFolder)
        {
            string requestFileName = file.FileName;
            string fileExtension = System.IO.Path.GetExtension(requestFileName).ToLower();
            if(fileName.IsNullOrEmpty())
            {
                fileName = requestFileName;
            }
            else
            {
                fileName = fileName + fileExtension;
            }
            if(BaseDirectory.IsNullOrEmpty())
            {
                BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }

            if(!BaseDirectory.EndsWith("\\"))
            {
                BaseDirectory = string.Format("{0}\\", BaseDirectory);
            }

            string fullDirectory = string.Empty;
            string url = string.Empty;
            if (useDateFolder)
            {
                fullDirectory = string.Format("{0}{1}\\{2}\\", BaseDirectory, subDir, DateTime.Now.ToString("yyyy-MM-dd"));
                url = string.Format("{0}/{1}/{2}", subDir, DateTime.Now.ToString("yyyy-MM-dd"),fileName);
            }
            else
            {
                fullDirectory = string.Format("{0}{1}\\", BaseDirectory, subDir);
                url = string.Format("{0}/{1}", subDir, fileName);
            }
            if(!Directory.Exists(fullDirectory))
            {
                Directory.CreateDirectory(fullDirectory);
            }
            file.SaveAs(fullDirectory + fileName);
            return url;
        }
    }
}
