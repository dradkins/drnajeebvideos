using System.Web;
using System.Web.Optimization;

namespace DrNajeeb.Web.API
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/bundles/app").Include(
                        "~/Scripts/tinycolor.js",
                        "~/Scripts/angular.js",
                        "~/Scripts/angular-route.js",
                        "~/Scripts/loading-bar.min.js",
                        "~/Scripts/angulardatepicker.js",
                        "~/Scripts/angular-ui/ui-bootstrap-tpls.js",
                        "~/Scripts/angular-color-picker.js",
                        "~/Scripts/angular-checkbox-list.js",
                        "~/Scripts/angular-animate.js",

                        "~/app/app.js",

                        "~/app/directives/menuactivator.js",

                        "~/app/services/SubscriptionService.js",
                        "~/app/services/CategoriesService.js",
                        "~/app/services/VideoService.js",
                        "~/app/services/CountryService.js",
                        "~/app/services/RolesService.js",
                        "~/app/services/UserService.js",
                        "~/app/services/DashboardService.js",

                        "~/app/controllers/SubscriptionController.js",
                        "~/app/controllers/CategoriesController.js",
                        "~/app/controllers/VideoController.js",
                        "~/app/controllers/UserController.js",
                        "~/app/controllers/DashboardController.js"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}
