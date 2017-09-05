using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;

namespace nuserv.Utility
{
    #region Usings

    #endregion

    public class HttpRouteDataResolver : IHttpRouteDataResolver
    {
        private readonly HttpConfiguration config;

        #region Public Methods and Operators

        public HttpRouteDataResolver(HttpConfiguration config)
        {
            this.config = config;
        }

        public IHttpRouteData Resolve()
        {
            if (HttpContext.Current != null)
            {
                var request = HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage;
                if (request != null)
                {
                    var routeData = this.config.Routes.GetRouteData(request);

                    return routeData;
                }
            }

            return null;
        }

        #endregion
    }
}