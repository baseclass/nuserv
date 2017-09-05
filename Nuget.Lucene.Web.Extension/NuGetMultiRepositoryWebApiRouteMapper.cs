using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData.Extensions;
using System.Web.Http.OData.Formatter;
using System.Web.Http.OData.Formatter.Deserialization;
using System.Web.Http.OData.Routing.Conventions;
using System.Web.Http.Routing;
using NuGet.Lucene.Web.OData.Batch;
using NuGet.Lucene.Web.OData.Formatter.Serialization;
using NuGet.Lucene.Web.OData.Routing;
using NuGet.Lucene.Web.OData.Routing.Conventions;

namespace NuGet.Lucene.Web.Extension
{
    #region Usings

    #endregion

    public class NuGetMultiRepositoryWebApiRouteMapper
    {
        #region Constructors and Destructors

        public NuGetMultiRepositoryWebApiRouteMapper(string pathPrefix, string repositoryPrefix)
        {
            PathPrefix = pathPrefix;
            this.repositoryPrefix = repositoryPrefix;
        }

        #endregion

        #region Fields

        private readonly string repositoryPrefix;

        #endregion

        #region Public Properties

        public string ODataRoutePath => repositoryPrefix + PathPrefix + "v2";

        public string PathPrefix { get; }

        #endregion

        #region Public Methods and Operators

        public void MapApiRoutes(HttpConfiguration config)
        {
            var routes = config.Routes;

            routes.MapHttpRoute(
                AspNet.WebApi.HtmlMicrodataFormatter.RouteNames.ApiDocumentation,
                PathPrefix + "api",
                new {controller = "NuGetMultiRepositoryDocumentation", action = "GetApiDocumentation"});

            routes.MapHttpRoute(
                AspNet.WebApi.HtmlMicrodataFormatter.RouteNames.TypeDocumentation,
                PathPrefix + "schema/{typeName}",
                new {controller = "NuGetDocumentation", action = "GetTypeDocumentation"});

            routes.MapHttpRoute(
                RouteNames.Indexing,
                PathPrefix + "indexing/{action}",
                new {controller = "Indexing"});

            routes.MapHttpRoute(
                RouteNames.Users.All,
                PathPrefix + "users",
                new {controller = "Users", action = "GetAllUsers"},
                new {httpMethod = new HttpMethodConstraint(HttpMethod.Get, HttpMethod.Options)});

            routes.MapHttpRoute(
                RouteNames.Users.GetUser,
                PathPrefix + "users/{*username}",
                new {controller = "Users", action = "Get"},
                new {username = ".+", method = new HttpMethodConstraint(HttpMethod.Get)});

            routes.MapHttpRoute(
                RouteNames.Users.PutUser,
                PathPrefix + "users/{*username}",
                new {controller = "Users", action = "Put"},
                new {username = ".+"});

            routes.MapHttpRoute(
                RouteNames.Users.DeleteUser,
                PathPrefix + "users/{*username}",
                new {controller = "Users", action = "Delete"},
                new {username = ".+"});

            routes.MapHttpRoute(
                RouteNames.Users.DeleteAll,
                PathPrefix + "users",
                new {controller = "Users", action = "DeleteAllUsers"},
                new {httpMethod = new HttpMethodConstraint(HttpMethod.Delete)});

            routes.MapHttpRoute(
                RouteNames.Users.GetAuthenticationInfo,
                PathPrefix + "session",
                new {controller = "Users", action = "GetAuthenticationInfo"});

            routes.MapHttpRoute(
                RouteNames.Users.GetRequiredAuthenticationInfo,
                PathPrefix + "authenticate",
                new {controller = "Users", action = "GetRequiredAuthenticationInfo"});

            routes.MapHttpRoute(
                RouteNames.TabCompletion.VS2013PackageIds,
                repositoryPrefix + PathPrefix + "v2/package-ids",
                new {controller = "TabCompletion", action = "GetMatchingPackages"});

            routes.MapHttpRoute(
                RouteNames.TabCompletion.VS2013PackageVersions,
                repositoryPrefix + PathPrefix + "v2/package-versions/{packageId}",
                new {controller = "TabCompletion", action = "GetPackageVersions"});

            routes.MapHttpRoute(RouteNames.TabCompletion.VS2015PackageIds,
                ODataRoutePath + "/package-ids",
                new {controller = "TabCompletion", action = "GetMatchingPackages"});

            routes.MapHttpRoute(RouteNames.TabCompletion.VS2015PackageVersions,
                ODataRoutePath + "/package-versions/{packageId}",
                new {controller = "TabCompletion", action = "GetPackageVersions"});

            routes.MapHttpRoute(
                RouteNames.Packages.Search,
                repositoryPrefix + PathPrefix + "packages",
                new {controller = "Packages", action = "Search"},
                new {httpMethod = new HttpMethodConstraint(HttpMethod.Get, HttpMethod.Options)});

            routes.MapHttpRoute(RouteNames.Packages.GetAvailableSearchFieldNames,
                repositoryPrefix + PathPrefix + "v2/packages/$searchable-fields",
                new {controller = "Packages", action = "GetAvailableSearchFieldNames"},
                new {httpMethod = new HttpMethodConstraint(HttpMethod.Get, HttpMethod.Options)});

            routes.MapHttpRoute(
                RouteNames.Packages.Upload,
                repositoryPrefix + PathPrefix + "v2/package",
                new {controller = "Packages"},
                new {httpMethod = new HttpMethodConstraint(HttpMethod.Put, HttpMethod.Options)});

            routes.MapHttpRoute(
                RouteNames.Packages.DownloadLatestVersion,
                repositoryPrefix + PathPrefix + "v2/package/{id}",
                new {controller = "Packages", action = "DownloadPackage"});

            routes.MapHttpRoute(
                RouteNames.Packages.Download,
                repositoryPrefix + PathPrefix + "v2/package/{id}/{version}",
                new {controller = "Packages", action = "DownloadPackage"},
                new {version = new SemanticVersionConstraint()});

            routes.MapHttpRoute(
                RouteNames.Packages.Info,
                repositoryPrefix + PathPrefix + "v2/package/{id}/{version}/info",
                new {controller = "Packages", action = "GetPackageInfo", version = RouteParameter.Optional},
                new
                {
                    httpMethod = new HttpMethodConstraint(HttpMethod.Get),
                    version = new OptionalSemanticVersionConstraint()
                });

            routes.MapHttpRoute(
                RouteNames.Packages.Delete,
                repositoryPrefix + PathPrefix + "v2/package/{id}/{version}",
                new {controller = "Packages", action = "DeletePackage"},
                new {version = new SemanticVersionConstraint()});
        }

        public void MapSymbolSourceRoutes(HttpConfiguration config)
        {
            var routes = config.Routes;


            routes.MapHttpRoute(RouteNames.Sources,
                repositoryPrefix + PathPrefix + "source/{id}/{version}/{*path}",
                new {controller = "SourceFiles"},
                new {version = new SemanticVersionConstraint()});


            routes.MapHttpRoute(RouteNames.Symbols.Settings,
                repositoryPrefix + PathPrefix + "symbol-settings",
                new {controller = "Symbols", action = "GetSettings"});

            routes.MapHttpRoute(RouteNames.Symbols.Upload,
                repositoryPrefix + PathPrefix + "symbols",
                new {controller = "Symbols"},
                new {httpMethod = new HttpMethodConstraint(HttpMethod.Put, HttpMethod.Options)});


            routes.MapHttpRoute(RouteNames.Symbols.GetFile,
                repositoryPrefix + PathPrefix + "symbols/{*path}",
                new {controller = "Symbols", action = "GetFile"});
        }


        public void MapDataServiceRoutes(HttpConfiguration config)
        {
            var builder = new NuGetWebApiODataModelBuilder();

            builder.Build();

            config.Formatters.InsertRange(0,
                ODataMediaTypeFormatters.Create(
                    new ODataPackageDefaultStreamAwareSerializerProvider(),
                    new DefaultODataDeserializerProvider()));

            var conventions = new List<IODataRoutingConvention>
            {
                new CompositeKeyRoutingConvention(),
                new CompositeKeyPropertyRoutingConvention(),
                new NonBindableActionRoutingConvention("PackagesOData"),
                new EntitySetCountRoutingConvention(),
                new NonBindableActionCountRoutingConvention("PackagesOData")
            };

            conventions.AddRange(ODataRoutingConventions.CreateDefault());

            conventions = conventions
                .Select(c => new ControllerAliasingODataRoutingConvention(c, "Packages", "PackagesOData"))
                .Cast<IODataRoutingConvention>().ToList();

            config.Routes.MapODataServiceRoute(
                RouteNames.Packages.Feed,
                ODataRoutePath,
                builder.Model,
                new CountODataPathHandler(),
                conventions,
                new HeaderCascadingODataBatchHandler(new NuGetWebApiRouteMapper.BatchHttpServer(config)));
        }

        #endregion
    }
}