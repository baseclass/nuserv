namespace NuGet.Lucene.Web.Extension
{
    #region Usings

    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Routing;

    using NuGet.Lucene.Web.DataServices;

    using Nuget.Lucene.Web.Extension;

    using HttpMethodConstraint = System.Web.Http.Routing.HttpMethodConstraint;

    #endregion

    public class NuGetMultiRepositoryWebApiRouteMapper
    {
        #region Fields

        private readonly string pathPrefix;

        private readonly string repositoryPrefix;

        #endregion

        #region Constructors and Destructors

        public NuGetMultiRepositoryWebApiRouteMapper(string pathPrefix, string repositoryPrefix)
        {
            this.pathPrefix = pathPrefix;
            this.repositoryPrefix = repositoryPrefix;
        }

        #endregion

        #region Public Properties

        public string ODataRoutePath
        {
            get
            {
                return this.repositoryPrefix + this.PathPrefix + "api/v2";
            }
        }

        public string PathPrefix
        {
            get
            {
                return this.pathPrefix;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void MapApiRoutes(HttpConfiguration config)
        {
            var routes = config.Routes;

            routes.MapHttpRoute(
                AspNet.WebApi.HtmlMicrodataFormatter.RouteNames.ApiDocumentation,
                this.pathPrefix + "api",
                new { controller = "NuGetMultiRepositoryDocumentation", action = "GetApiDocumentation" });

            routes.MapHttpRoute(
                AspNet.WebApi.HtmlMicrodataFormatter.RouteNames.TypeDocumentation,
                this.pathPrefix + "api/schema/{typeName}",
                new { controller = "NuGetDocumentation", action = "GetTypeDocumentation" });

            routes.MapHttpRoute(
                RouteNames.Indexing,
                this.pathPrefix + "api/indexing/{action}",
                new { controller = "Indexing" });

            routes.MapHttpRoute(
                RouteNames.Users.All,
                this.pathPrefix + "api/users",
                new { controller = "Users", action = "GetAllUsers" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Get, HttpMethod.Options) });

            routes.MapHttpRoute(
                RouteNames.Users.GetUser,
                this.pathPrefix + "api/users/{*username}",
                new { controller = "Users", action = "Get" },
                new { username = ".+", method = new HttpMethodConstraint(HttpMethod.Get) });

            routes.MapHttpRoute(
                RouteNames.Users.PutUser,
                this.pathPrefix + "api/users/{*username}",
                new { controller = "Users", action = "Put" },
                new { username = ".+" });

            routes.MapHttpRoute(
                RouteNames.Users.DeleteUser,
                this.pathPrefix + "api/users/{*username}",
                new { controller = "Users", action = "Delete" },
                new { username = ".+" });

            routes.MapHttpRoute(
                RouteNames.Users.DeleteAll,
                this.pathPrefix + "api/users",
                new { controller = "Users", action = "DeleteAllUsers" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });

            routes.MapHttpRoute(
                RouteNames.Users.GetAuthenticationInfo,
                this.pathPrefix + "api/session",
                new { controller = "Users", action = "GetAuthenticationInfo" });

            routes.MapHttpRoute(
                RouteNames.Users.GetRequiredAuthenticationInfo,
                this.pathPrefix + "api/authenticate",
                new { controller = "Users", action = "GetRequiredAuthenticationInfo" });

            routes.MapHttpRoute(
                RouteNames.TabCompletionPackageIds,
                this.repositoryPrefix + this.pathPrefix + "api/v2/package-ids",
                new { controller = "TabCompletion", action = "GetMatchingPackages" });

            routes.MapHttpRoute(
                RouteNames.TabCompletionPackageVersions,
                this.repositoryPrefix + this.pathPrefix + "api/v2/package-versions/{packageId}",
                new { controller = "TabCompletion", action = "GetPackageVersions" });

            routes.MapHttpRoute(
                RouteNames.Packages.Search,
                this.repositoryPrefix + this.pathPrefix + "api/packages",
                new { controller = "Packages", action = "Search" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Get, HttpMethod.Options) });

            routes.MapHttpRoute(
                RouteNames.Packages.Upload,
                this.repositoryPrefix + this.pathPrefix + "package",
                new { controller = "Packages" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Put, HttpMethod.Options) });

            routes.MapHttpRoute(
                RouteNames.Packages.DownloadLatestVersion,
                this.repositoryPrefix + this.pathPrefix + "package/{id}",
                new { controller = "Packages", action = "DownloadPackage" });

            routes.MapHttpRoute(
                RouteNames.Packages.Download,
                this.repositoryPrefix + this.pathPrefix + "package/{id}/{version}",
                new { controller = "Packages", action = "DownloadPackage" },
                new { version = new SemanticVersionConstraint() });

            routes.MapHttpRoute(
                RouteNames.Packages.Info,
                this.repositoryPrefix + this.pathPrefix + "package/{id}/{version}/info",
                new { controller = "Packages", action = "GetPackageInfo", version = "" },
                new
                {
                    httpMethod = new HttpMethodConstraint(HttpMethod.Get),
                    version = new OptionalSemanticVersionConstraint()
                });

            routes.MapHttpRoute(
                RouteNames.Packages.Delete,
                this.repositoryPrefix + this.pathPrefix + "package/{id}/{version}",
                new { controller = "Packages", action = "DeletePackage" },
                new { version = new SemanticVersionConstraint() });
        }

        public void MapDataServiceRoutes(RouteCollection routes)
        {
            var dataServiceHostFactory = new RewriteBaseUrlNinjectDataServiceHostFactory();

            var serviceRoute = new DynamicServiceRoute(
                this.ODataRoutePath,
                RouteNames.PackageFeedRouteValues,
                null,
                dataServiceHostFactory,
                typeof(PackageDataService));

            routes.Add(RouteNames.Packages.Feed, serviceRoute);
        }

        #endregion
    }
}