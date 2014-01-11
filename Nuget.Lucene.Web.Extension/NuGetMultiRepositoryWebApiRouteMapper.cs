using System.Net.Http;
using System.ServiceModel.Activation;
using System.Web.Http;
using System.Web.Routing;
using Ninject.Extensions.Wcf;
using NuGet.Lucene.Web.DataServices;
using NuGet.Lucene;
using NuGet.Lucene.Web;
using NuGet.Lucene.Web.Authentication;
using NuGet.Lucene.Web.Models;
using NuGet.Lucene.Web.Modules;
using HttpMethodConstraint = System.Web.Http.Routing.HttpMethodConstraint;

namespace NuGet.Lucene.Web.Extension
{
    public class NuGetMultiRepositoryWebApiRouteMapper
    {
        private readonly string pathPrefix;
        private readonly string repositoryPrefix;

        public NuGetMultiRepositoryWebApiRouteMapper(string pathPrefix, string repositoryPrefix)
        {
            this.pathPrefix = pathPrefix;
            this.repositoryPrefix = repositoryPrefix;
        }

        public void MapApiRoutes(HttpConfiguration config)
        {
            var routes = config.Routes;
            
            routes.MapHttpRoute(AspNet.WebApi.HtmlMicrodataFormatter.RouteNames.ApiDocumentation,
                                pathPrefix,
                                new { controller = "NuGetMultiRepositoryDocumentation", action = "GetApiDocumentation" });

            routes.MapHttpRoute(AspNet.WebApi.HtmlMicrodataFormatter.RouteNames.TypeDocumentation,
                                pathPrefix + "schema/{typeName}",
                                new { controller = "NuGetDocumentation", action = "GetTypeDocumentation" });
            
            routes.MapHttpRoute(RouteNames.Indexing,
                                pathPrefix + "indexing/{action}",
                                new { controller = "Indexing" });
            
            routes.MapHttpRoute(RouteNames.Users.All,
                                pathPrefix + "users",
                                new { controller = "Users", action = "GetAllUsers" },
                                new { httpMethod = new HttpMethodConstraint(HttpMethod.Get, HttpMethod.Options) });

            routes.MapHttpRoute(RouteNames.Users.GetUser,
                                pathPrefix + "users/{*username}",
                                new { controller = "Users", action = "Get" },
                                new { username = ".+", method = new HttpMethodConstraint(HttpMethod.Get) });

            routes.MapHttpRoute(RouteNames.Users.PutUser,
                                pathPrefix + "users/{*username}",
                                new { controller = "Users", action = "Put" },
                                new { username = ".+" });

            routes.MapHttpRoute(RouteNames.Users.DeleteUser,
                                pathPrefix + "users/{*username}",
                                new { controller = "Users", action = "Delete" },
                                new { username = ".+" });

            routes.MapHttpRoute(RouteNames.Users.DeleteAll,
                                pathPrefix + "users",
                                new { controller = "Users", action = "DeleteAllUsers" },
                                new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });

            routes.MapHttpRoute(RouteNames.Users.GetAuthenticationInfo,
                                pathPrefix + "session",
                                new { controller = "Users", action = "GetAuthenticationInfo" });

            routes.MapHttpRoute(RouteNames.Users.GetRequiredAuthenticationInfo,
                                pathPrefix + "authenticate",
                                new { controller = "Users", action = "GetRequiredAuthenticationInfo" });
            
            routes.MapHttpRoute(RouteNames.TabCompletionPackageIds,
                                repositoryPrefix + pathPrefix + "v2/package-ids",
                                new { controller = "TabCompletion", action = "GetMatchingPackages" });

            routes.MapHttpRoute(RouteNames.TabCompletionPackageVersions,
                                repositoryPrefix + pathPrefix+ "v2/package-versions/{packageId}",
                                new { controller = "TabCompletion", action = "GetPackageVersions" });
            
            routes.MapHttpRoute(RouteNames.Packages.Search,
                                repositoryPrefix + pathPrefix + "packages",
                                new { controller = "Packages", action = "Search" },
                                new { httpMethod = new HttpMethodConstraint(HttpMethod.Get, HttpMethod.Options) });
            
            routes.MapHttpRoute(RouteNames.Packages.Upload,
                                repositoryPrefix + pathPrefix + "packages",
                                new { controller = "Packages" },
                                new { httpMethod = new HttpMethodConstraint(HttpMethod.Put, HttpMethod.Options) });
            
            routes.MapHttpRoute(RouteNames.Packages.DownloadLatestVersion,
                                repositoryPrefix + pathPrefix + "packages/{id}/content",
                                new { controller = "Packages", action = "DownloadPackage" });

            routes.MapHttpRoute(RouteNames.Packages.Download,
                                repositoryPrefix + pathPrefix + "packages/{id}/{version}/content",
                                new { controller = "Packages", action = "DownloadPackage" },
                                new { version = new SemanticVersionConstraint() });

            routes.MapHttpRoute(RouteNames.Packages.Info,
                                repositoryPrefix + pathPrefix + "packages/{id}/{version}",
                                new { controller = "Packages", action = "GetPackageInfo", version = "" },
                                new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), version = new OptionalSemanticVersionConstraint() });
            
            routes.MapHttpRoute(RouteNames.Packages.Delete,
                                repositoryPrefix + pathPrefix + "packages/{id}/{version}",
                                new { controller = "Packages", action = "DeletePackage" },
                                new { version = new SemanticVersionConstraint() });
        }

        public void MapDataServiceRoutes(RouteCollection routes)
        {
            var dataServiceHostFactory = new NinjectDataServiceHostFactory();

            var serviceRoute = new ServiceRoute(ODataRoutePath, dataServiceHostFactory, typeof(PackageDataService))
            {
                Defaults = RouteNames.PackageFeedRouteValues,
                Constraints = RouteNames.PackageFeedRouteValues
            };

            routes.Add(RouteNames.Packages.Feed, serviceRoute);
        }

        public string PathPrefix { get { return pathPrefix; } }
        public string ODataRoutePath { get { return PathPrefix + "odata"; } }
        public string SignalrRoutePath { get { return PathPrefix + "signalr"; } }
    }
}