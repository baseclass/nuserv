namespace nuserv.Utility
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http.Routing;

    #endregion

    public class HttpRouteDataResolver : IHttpRouteDataResolver
    {
        #region Public Methods and Operators

        public IHttpRouteData Resolve()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items.Contains(""))
                {
                    var requestMessage = HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage;

                    if (requestMessage != null)
                    {
                        return requestMessage.GetRouteData();
                    }
                }
                else
                {
                    return new RouteData(HttpContext.Current.Request.RequestContext.RouteData);
                }
            }

            return null;
        }

        #endregion

        private class RouteData : IHttpRouteData
        {
            #region Fields

            private readonly System.Web.Routing.RouteData originalRouteData;

            #endregion

            #region Constructors and Destructors

            public RouteData(System.Web.Routing.RouteData routeData)
            {
                if (routeData == null)
                {
                    throw new ArgumentNullException("routeData");
                }
                this.originalRouteData = routeData;
                this.Route = null;
            }

            #endregion

            #region Public Properties

            public IHttpRoute Route { get; private set; }

            public IDictionary<string, object> Values
            {
                get
                {
                    return this.originalRouteData.Values;
                }
            }

            #endregion
        }
    }
}