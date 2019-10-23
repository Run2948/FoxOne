using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FoxOne.Core
{
    public static class StringExtension
    {
        public static string Substring(this string str, int length, string endOf)
        {
            if (!string.IsNullOrEmpty(str) && ((length > 0) && (length < str.Length)))
            {
                return (str.Substring(0, length) + endOf);
            }
            return str;
        }

        public static string FormatTo(this string str, params object[] args)
        {
            return string.Format(str, args);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNotNullOrEmpty(this string str)
        {
            return !str.IsNullOrEmpty();
        }

        public static string GetPY(this string str)
        {
            try
            {
                string errStr = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ(){}[]-+=\\/,.!@#$%^&*~;:'";
                if (errStr.Contains(str))
                {
                    return str;
                }
                byte[] array = new byte[2];
                array = System.Text.Encoding.Default.GetBytes(str);
                int i = (short)(array[0] - '\0') * 256 + ((short)(array[1] - '\0'));
                if (i < 0xB0A1) return "*";
                if (i < 0xB0C5) return "A";
                if (i < 0xB2C1) return "B";
                if (i < 0xB4EE) return "C";
                if (i < 0xB6EA) return "D";
                if (i < 0xB7A2) return "E";
                if (i < 0xB8C1) return "F";
                if (i < 0xB9FE) return "G";
                if (i < 0xBBF7) return "H";
                if (i < 0xBFA6) return "J";
                if (i < 0xC0AC) return "K";
                if (i < 0xC2E8) return "L";
                if (i < 0xC4C3) return "M";
                if (i < 0xC5B6) return "N";
                if (i < 0xC5BE) return "O";
                if (i < 0xC6DA) return "P";
                if (i < 0xC8BB) return "Q";
                if (i < 0xC8F6) return "R";
                if (i < 0xCBFA) return "S";
                if (i < 0xCDDA) return "T";
                if (i < 0xCEF4) return "W";
                if (i < 0xD1B9) return "X";
                if (i < 0xD4D1) return "Y";
                if (i < 0xD7FA) return "Z";
                return "*";
            }
            catch (Exception)
            {
                return "*";
            }
        }

        public static string StripHTML(this string strHtml)
        {
            string[] aryReg =
        {
          @"<script[^>]*?>.*?</script>",@"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[""'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>", @"([\r\n])[\s]+", @"&(quot|#34);", @"&(amp|#38);", @"&(lt|#60);", @"&(gt|#62);", @"&(nbsp|#160);", @"&(iexcl|#161);", @"&(cent|#162);", @"&(pound|#163);",@"&(copy|#169);", @"&#(\d+);", @"-->", @"<!--.*\n"
        };
            string[] aryRep =
        {
          "", "", "", "\"", "&", "<", ">", "   ", "\xa1",  //chr(161),
          "\xa2",  //chr(162),
          "\xa3",  //chr(163),
          "\xa9",  //chr(169),
          "", "\r\n", ""
        };
            string newReg = aryReg[0];
            string strOutput = strHtml;
            for (int i = 0; i < aryReg.Length; i++)
            {
                Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                strOutput = regex.Replace(strOutput, aryRep[i]);
            }
            strOutput.Replace("<", "");
            strOutput.Replace(">", "");
            strOutput.Replace("\r\n", "");
            return strOutput;
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 从字符串结尾处起向左获取LEN个字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len">要获取的字符长度</param>
        /// <returns></returns>
        public static string Right(this string str, int len)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (str.Length < len)
            {
                throw new ArgumentException("len argument can not be bigger than given string's length!");
            }

            return str.Substring(str.Length - len, len);
        }

        public static T ToEnum<T>(this string str) where T : struct
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            return (T)Enum.Parse(typeof(T), str);
        }

        public static T ToEnum<T>(this string str, bool ignoreCase) where T : struct
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            return (T)Enum.Parse(typeof(T), str, ignoreCase);
        }
    }
}
