using System.Web;
using System.Web.Optimization;

namespace GoGoNihon_MVC
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                        "~/bower_components/jquery/dist/jquery.min.js",
                        "~/bower_components/bootstrap/dist/js/bootstrap.min.js",
                        "~/Scripts/slider.js",
                         "~/Scripts/js.js"));

            bundles.Add(new ScriptBundle("~/bundles/js-frontAdmin").Include(
                        "~/bower_components/jquery/dist/jquery.min.js",
                        "~/bower_components/bootstrap/dist/js/bootstrap.min.js",
                        "~/Scripts/slider.js",
                        "~/Scripts/tinymce/tinymce.min.js",
                         "~/Scripts/js.js"));


            bundles.Add(new ScriptBundle("~/bundles/admin-js").Include(
                        "~/bower_components/jquery/dist/jquery.min.js",
                        "~/bower_components/bootstrap/dist/js/bootstrap.min.js",
                        "~/Scripts/tinymce/tinymce.min.js",
                         "~/Scripts/admin-js.js"));

            bundles.Add(new StyleBundle("~/Content/Admincss").Include(
                      "~/bower_components/bootstrap/less/bootstrap.css",
                      "~/Content/admin.css",
                      "~/Content/icons.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/bower_components/bootstrap/less/bootstrap.css")
                      .Include("~/Content/style.css")
                      .Include("~/Content/slider.css")
                        .Include("~/Content/icons.css", new CssRewriteUrlTransform())
                        );
        }
    }
}
