namespace nuserv
{
    #region Usings

    using System.Web.Optimization;

    #endregion

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862

        #region Public Methods and Operators

        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/bower_components/jquery/dist/jquery.min.js"));

            bundles.Add(
                new ScriptBundle("~/bundles/angular").Include("~/bower_components/angular/angular.min.js")
                    .Include("~/bower_components/angular-route/angular-route.min.js"));

            bundles.Add(
                new ScriptBundle("~/bundles/bootstrap").Include("~/bower_components/bootstrap/dist/js/bootstrap.min.js"));

            bundles.Add(
                new StyleBundle("~/Content/css").Include(
                    "~/bower_components/bootstrap/dist/css/bootstrap.css",
                    "~/Content/site.css",
                    "~/bower_components/font-awesome/css/font-awesome.min.css"));
        }

        #endregion
    }
}