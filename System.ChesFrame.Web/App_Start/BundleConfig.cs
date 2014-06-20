using System.Web;
using System.Web.Optimization;

namespace System.ChesFrame.Web
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //juery
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Content/Scripts/common/jquery-{version}.js"));

            //jquery验证
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Content/Scripts/common/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            //检查浏览器对HTML的支持
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Content/Scripts/common/modernizr-*"));


            //bootstrap js
            bundles.Add(new ScriptBundle("~/bundles/jquerybootstrap").Include(
                        "~/Content/Scripts/common/bootstrap.js"));

            //bootstrap css
            bundles.Add(new StyleBundle("~/bundles/cssbootstrap").Include(
                       "~/Content/CSS/css/bootstrap.css",
                        "~/Content/CSS/css/bootstrap-theme.css",
                        //"~/Content/CSS/css/bootstrap-theme.css.map",
                        "~/Content/CSS/css/bootstrap.localization.css"));//最后

            //location js
            bundles.Add(new ScriptBundle("~/bundles/jschesframe").Include(
                        "~/Content/Scripts/common/chesFrame.js"));

            //location css
            bundles.Add(new StyleBundle("~/bundles/csschesframe").Include(
                        "~/Content/CSS/css/chesFrame.css"));

            bundles.Add(new StyleBundle("~/bundles/indexlayout").Include(
                       "~/Content/CSS/css/indexlayout.css"));

            bundles.Add(new StyleBundle("~/bundles/pileMenulayoutcss").Include(
                       "~/Content/CSS/css/pileMenulayout.css"));

            /*~/bundles/jquery
             * ~/bundles/modernizr
             * ~/bundles/jquerybootstrap
             * ~/bundles/cssbootstrap
             * ~/bundles/chesframe
             * ~/bundles/chesframe
             */

            //在顺序上 本地的放在最后

            //Layer 第三方弹出窗口或者弹出层
            bundles.Add(new ScriptBundle("~/bundles/layerjs").Include(
                        "~/Content/layer/layer.min.js"));
            bundles.Add(new StyleBundle("~/bundles/layercss").Include(
                        "~/Content/layer/skin/layer.css"));


           
        }
    }
}
