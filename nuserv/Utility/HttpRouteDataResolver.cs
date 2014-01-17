namespace nuserv.Utility
{
    using System.Net.Http;
    using System.Web;
    using System.Web.Http.Routing;

    public class HttpRouteDataResolver : IHttpRouteDataResolver
    {
        #region Public Methods and Operators

        public IHttpRouteData Resolve()
        {
            if (HttpContext.Current != null)
            {
                var requestMessage = HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage;

                return requestMessage.GetRouteData();
            }

            return null;
        }

        #endregion
    }
}