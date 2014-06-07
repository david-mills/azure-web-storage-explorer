using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace Azure.Diagnostics.Website
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                         "~/content/bootstrap.css",
                         "~/content/bootstrap-theme.min.css",
                         "~/content/site.css",
                         "~/content/datetimepicker.css",
                         "~/content/spinner.css",
                         "~/content/x-editable.css"));

            bundles.Add(new ScriptBundle("~/bundles/libaries-top").Include(
                         "~/Scripts/jquery/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/libaries-bottom").Include(
                         "~/Scripts/angular/angular.js",
                         "~/Scripts/angular/angular-cookies.js",
                         "~/Scripts/angular/angular-route.js",
                         "~/Scripts/bootstrap/bootstrap.js",
                         "~/Scripts/directives/moment.min.js",
                         "~/Scripts/directives/datetimepicker.js", 
                         "~/Scripts/directives/angular-charts.js", 
                         "~/Scripts/directives/checklist-model.js", 
                         "~/Scripts/directives/spinner.js", 
                         "~/Scripts/directives/table-pager.js",
                         "~/Scripts/directives/x-editable.js"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                         "~/Scripts/app.js",
                         "~/Scripts/deployments.js",
                         "~/Scripts/perfcounters.js",
                         "~/Scripts/storage.js"));

            // Code removed for clarity.
            BundleTable.EnableOptimizations = false;
        }
    }
}