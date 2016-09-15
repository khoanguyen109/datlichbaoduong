using System.Web.Optimization;

namespace MaintenanceSchedule
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //Cross domain url
            //var url = "http://carspa.vn/";

            //Bundle Css
            bundles.Add(new StyleBundle("~/Css/Layout").Include(
                      "~/Assets/css/home/rw-style_responsive_new.css",
                      "~/Assets/css/home/ui.css",
                      "~/Assets/css/home/site.css",
                      "~/Assets/css/home/home.css",
                      "~/Assets/css/home/rw-core.0e59a.1.css",
                      "~/Assets/css/home/cta.css",
                      "~/Assets/css/home/crm_header_new.css",
                      "~/Assets/css/home/header_redesigned.css",
                      "~/Assets/css/home/footer_more.css"));

            bundles.Add(new StyleBundle("~/Css/Alert").Include(
                      "~/Assets/css/alert/notify.css",
                      "~/Assets/css/alert/prettify.css"));

            bundles.Add(new StyleBundle("~/Css/Home/Form").Include(
                      "~/Assets/css/home/form.css"));

            //bundles.Add(new StyleBundle("~/Css/Service/_Detail").Include(
            //          "~/Assets/css/font-awesome.css",
            //          "~/Assets/css/service/service_view.css"));

            bundles.Add(new StyleBundle("~/Css/Footer").Include(
                      "~/Assets/css/service/sppagebuilder.css",
                      "~/Assets/css/service/separated.css",
                      "~/Assets/css/service/bootstrap.min.css",
                      "~/Assets/css/service/template.css",
                      "~/Assets/css/service/preset1.css",
                      "~/Assets/css/home/footer.css"));

            bundles.Add(new StyleBundle("~/Css/Service/_Index").Include(
                      "~/Assets/css/service/sppagebuilder.css",
                      "~/Assets/css/service/separated.css",
                      "~/Assets/css/service/fonts_googleapis.css",
                      "~/Assets/css/service/bootstrap.min.css",
                      "~/Assets/css/service/template.css",
                      "~/Assets/css/service/preset1.css",
                      "~/Assets/css/service/custom.css",
                      "~/Assets/css/service/service_index.css"));

            bundles.Add(new StyleBundle("~/Css/Service/_Detail").Include(
                      "~/Assets/css/service/fonts_googleapis.css",
                      "~/Assets/css/service/bootstrap.min.css",
                      "~/Assets/css/service/legacy.css",
                      "~/Assets/css/service/template.css",
                      "~/Assets/css/service/preset1.css",
                      "~/Assets/css/service/custom.css",
                      "~/Assets/css/font-awesome.css",
                      "~/Assets/css/service/service_view.css"));

            bundles.Add(new StyleBundle("~/Css/Common/Admin").Include(
                      "~/Assets/css/bootstrap.css",
                      "~/Assets/css/jquery.bootgrid.css",
                      "~/Assets/css/bootstrap-select.css",
                      "~/Assets/css/bootstrap-datetimepicker.css",
                      "~/Assets/css/font-awesome.css",
                      "~/Assets/css/admin/admin.css",
                      "~/Assets/css/toastr.css",
                      "~/Assets/css/formValidation.css"));

            bundles.Add(new StyleBundle("~/Css/SignIn").Include(
                      "~/Assets/css/bootstrap.css",
                      "~/Assets/css/admin/signin/form-elements.css",
                      "~/Assets/css/admin/signin/style.css",
                      "~/Assets/css/formValidation.css"));

            //Bundle Javascript
            bundles.Add(new ScriptBundle("~/Js/Jquery").Include(
                      "~/Assets/js/jquery-2.2.3.js"));

            bundles.Add(new ScriptBundle("~/Js/JqueryValidation").Include(
                      "~/Assets/js/jquery.validate.js"));

            bundles.Add(new ScriptBundle("~/Js/JqueryTemplate").Include(
                      "~/Assets/js/jquery.tmpl.js"));

            bundles.Add(new ScriptBundle("~/Js/Alert").Include(
                      //"~/Assets/js/alert/notify.js",
                      //"~/Assets/js/alert/prettify.js",
                      "~/Assets/js/alert/notifyBootstrap.js"));

            bundles.Add(new ScriptBundle("~/Js/Layout").Include(
                      "~/Assets/js/layout.js"));

            bundles.Add(new ScriptBundle("~/Js/Home/Form").Include(
                      "~/Assets/js/bootstrap.js",
                      "~/Assets/js/moment.js",
                      "~/Assets/js/toastr.js",
                      "~/Assets/js/scripts.js",
                      "~/Assets/js/form.js"));

            bundles.Add(new ScriptBundle("~/Js/Service/_Index").Include(
                      "~/Assets/js/service.js"));

            bundles.Add(new ScriptBundle("~/Js/Common/Admin").Include(
                     "~/Assets/js/jquery-2.2.3.js",
                     "~/Assets/js/jquery.validate.js",
                     "~/Assets/js/jquery.validate.unobtrusive.js",
                     "~/Assets/js/bootstrap.js",
                     "~/Assets/js/respond.js",
                     "~/Assets/js/jquery.form.js",
                     "~/Assets/js/moment.js",
                     "~/Assets/js/bootstrap-datetimepicker.js",
                     "~/Assets/js/bootstrap-select.js",
                     "~/Assets/js/toastr.js",
                     "~/Assets/js/jquery.bootgrid.js",
                     "~/Assets/js/scripts.js",
                     "~/Assets/js/ajax.js",
                     "~/Assets/js/admin/admin-service.js",
                     "~/Assets/js/formValidation.js",
                     "~/Assets/js/bootstrapFramework.js"));

            bundles.Add(new ScriptBundle("~/Js/SignIn").Include(
                     "~/Assets/js/jquery-2.2.3.js",
                     "~/Assets/js/jquery.validate.js",
                     "~/Assets/js/jquery.validate.unobtrusive.js",
                     "~/Assets/js/bootstrap.js",
                     "~/Assets/js/formValidation.js",
                     "~/Assets/js/bootstrapFramework.js",
                     "~/Assets/js/admin/signin/jquery.backstretch.js",
                     "~/Assets/js/admin/signin/sign-in.js"));
        }
    }
}
