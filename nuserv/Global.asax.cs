namespace nuserv
{
    #region Usings

    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    using AspNet.WebApi.HtmlMicrodataFormatter;

    using NuGet.Lucene.Web.Formatters;

    #endregion

    public class MvcApplication : System.Web.HttpApplication
    {
        #region Methods

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        #endregion
    }
}