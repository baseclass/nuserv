namespace Nuget.Lucene.Web.Extension
{
    #region Usings

    using System.Linq;

    using Ninject.Extensions.Wcf;

    #region Usings

    using System;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Web;
    using System.Web;
    using System.Web.Routing;

    #endregion

    #endregion

    //Thanks to:
    //http://blog.maartenballiauw.be/post/2011/11/08/Rewriting-WCF-OData-Services-base-URL-with-load-balancing-reverse-proxy.aspx
    //http://blog.maartenballiauw.be/post/2011/05/09/Using-dynamic-WCF-service-routes.aspx
    //http://blogs.msdn.com/b/astoriateam/archive/2010/07/21/odata-and-authentication-part-6-custom-basic-authentication.aspx
    //https://github.com/SymbolSource/SymbolSource.Community/blob/master/Sources/SymbolSource.Gateway.NuGet.Core/OData.cs

    public class RewriteBaseUrlMessageInspector : IDispatchMessageInspector
    {
        #region Public Methods and Operators

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            if (WebOperationContext.Current != null
                && WebOperationContext.Current.IncomingRequest.UriTemplateMatch != null)
            {
                var baseUriBuilder = new UriBuilder(
                    WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri);
                var requestUriBuilder =
                    new UriBuilder(WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri);

                var routeData = DynamicServiceRoute.GetCurrentRouteData();
                var route = routeData.Route as Route;
                if (route != null)
                {
                    string servicePath = route.Url;
                    foreach (var routeValue in routeData.Values)
                    {
                        if (routeValue.Value != null)
                        {
                            servicePath = servicePath.Replace("{" + routeValue.Key + "}", routeValue.Value.ToString());
                        }
                    }

                    servicePath = servicePath.Replace("{*servicePath}", string.Empty);

                    if (!servicePath.StartsWith("/"))
                    {
                        servicePath = "/" + servicePath;
                    }

                    if (!servicePath.EndsWith("/"))
                    {
                        servicePath = servicePath + "/";
                    }

                    requestUriBuilder.Path = requestUriBuilder.Path.Replace(baseUriBuilder.Path, servicePath);
                    requestUriBuilder.Host = baseUriBuilder.Host;
                    baseUriBuilder.Path = servicePath;
                }

                OperationContext.Current.IncomingMessageProperties["MicrosoftDataServicesRootUri"] = baseUriBuilder.Uri;
                OperationContext.Current.IncomingMessageProperties["MicrosoftDataServicesRequestUri"] =
                    requestUriBuilder.Uri;
            }

            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
        }

        #endregion
    }

    public class RewriteBaseUrlNinjectDataServiceHostFactory : NinjectDataServiceHostFactory
    {
        #region Public Methods and Operators

        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            var serviceHostBase = base.CreateServiceHost(constructorString, baseAddresses);

            foreach (var channelDispatcher in serviceHostBase.ChannelDispatchers.Cast<ChannelDispatcher>())
            {
                foreach (var endpointDispatcher in channelDispatcher.Endpoints)
                {
                    endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new RewriteBaseUrlMessageInspector());
                }
            }

            return serviceHostBase;
        }

        #endregion
    }

    public class DynamicServiceRoute : RouteBase, IRouteHandler
    {
        #region Fields

        private readonly Route innerRoute;

        private readonly ServiceRoute innerServiceRoute;

        private readonly string virtualPath;

        #endregion

        #region Constructors and Destructors

        public DynamicServiceRoute(
            string pathPrefix,
            object defaults,
            string[] namespaces,
            ServiceHostFactoryBase serviceHostFactory,
            Type serviceType)
        {
            if (pathPrefix.IndexOf("{*", StringComparison.Ordinal) >= 0)
            {
                throw new ArgumentException("Path prefix can not include catch-all route parameters.", "pathPrefix");
            }

            if (!pathPrefix.EndsWith("/"))
            {
                pathPrefix += "/";
            }

            pathPrefix += "{*servicePath}";

            this.virtualPath = serviceType.FullName + "-" + Guid.NewGuid().ToString() + "/";
            this.innerServiceRoute = new ServiceRoute(this.virtualPath, serviceHostFactory, serviceType);

            this.innerRoute = new Route(pathPrefix, new RouteValueDictionary(defaults), this)
                              {
                                  DataTokens =
                                      new RouteValueDictionary
                                      ()
                              };

            if ((namespaces != null) && (namespaces.Length > 0))
            {
                this.innerRoute.DataTokens["Namespaces"] = namespaces;
            }
        }

        #endregion

        #region Public Properties

        public Route InnerRoute
        {
            get
            {
                return this.innerRoute;
            }
        }

        #endregion

        #region Public Methods and Operators

        public static RouteData GetCurrentRouteData()
        {
            if (HttpContext.Current != null)
            {
                var wrapper = new HttpContextWrapper(HttpContext.Current);
                return wrapper.Request.RequestContext.RouteData;
            }
            return null;
        }

        public IHttpHandler GetHttpHandler(System.Web.Routing.RequestContext requestContext)
        {
            requestContext.HttpContext.RewritePath(
                "~/" + this.virtualPath + requestContext.RouteData.Values["servicePath"],
                true);
            return this.innerServiceRoute.RouteHandler.GetHttpHandler(requestContext);
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            return this.innerRoute.GetRouteData(httpContext);
        }

        public override VirtualPathData GetVirtualPath(
            System.Web.Routing.RequestContext requestContext,
            RouteValueDictionary values)
        {
            return null;
        }

        #endregion
    }
}