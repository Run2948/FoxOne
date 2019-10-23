using System.Web;
using System.Web.Optimization;

namespace FoxOne.Web
{
    public class BundleConfig
    {
        // 有关 Bundling 的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/style/common.css")
                .Include("~/Styles/reset.css")
                .Include("~/Styles/icon.css")
                .Include("~/Styles/common.css")
                .Include("~/Styles/zTreeStyle.css")
                .Include("~/Styles/easyui.css")
                .Include("~/Styles/default.css"));
            bundles.Add(new ScriptBundle("~/script/common.js")
                .Include("~/Scripts/jquery-1.8.2.js")
                .Include("~/Scripts/layout/jquery.easyui.js")
                .Include("~/Scripts/zTree/jquery.ztree.all-3.5.js")
                .Include("~/Scripts/jquery.blockui.js")
                .Include("~/Scripts/modal.js")
                .Include("~/Scripts/Validator/jquery.validation.js")
                .Include("~/Scripts/common.js")
                .Include("~/Scripts/datepicker/WdatePicker.js"));
            bundles.Add(new ScriptBundle("~/script/widget.js").Include("~/Scripts/widget/*.js"));
        }
    }
}