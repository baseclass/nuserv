namespace nuserv
{
    #region Usings

    using System.Web.Http;

    #endregion

    public static class WebApiConfig
    {
        #region Public Methods and Operators

        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/repository/{id}",
                defaults: new { controller = "Repository", id = RouteParameter.Optional });
        }

        #endregion
    }
}