using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Controls
{
    public static class TemplateGenerator
    {
        public static string GetDefaultFormTemplate()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("<form action=\"{0}\" defaultForm=\"true\" method=\"post\" class=\"{1}\" enctype=\"multipart/form-data\">");
            result.AppendLine("{2}{3}");
            result.AppendLine("<div class=\"form-group\"><label>&nbsp;</label>{4}</div>");
            result.AppendLine("</form>");
            return result.ToString();
        }

        public static string GetSearchFormTemplate()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("<form searchForm=\"true\" method=\"post\" class=\"{0}\">");
            result.AppendLine("<div class=\"input-control\">");
            result.AppendLine("{1}");
            result.AppendLine("</div>");
            result.AppendLine("{2}");
            result.AppendLine("</form>");
            return result.ToString();
        }

        public static string GetFormFieldTemplate()
        {
            return "<div class=\"form-group\" description=\"{3}\"><label for=\"{0}\">{1}：</label>\n{2}\n</div>";
        }

        public static string GetPagerTemplate()
        {
            return "<div class=\"data-pager-left\"><ul>{0}</ul></div><div class=\"data-pager-right\"><span class=\"data-pager-right-number\">每页</span><ul> {1} </ul><span class=\"data-pager-right-number\">行</span></div>";
        }

        public static string GetPagerItemTemplate()
        {
            return "<li><a pageIndex=\"{0}\" {1}>{2}</a></li>";
        }

        public static string GetPagerSizeTemplate() 
        {
            return "<li><a pageSize=\"{0}\" {1}>{2}</a></li>";
        }
    }
}
