using System.IO;
using System.Net.Http.Formatting;
using System.Web.Hosting;
using Autofac;
using Autofac.Integration.WebApi;
using Lucene.Net.Linq;
using Lucene.Net.Store;
using Lucene.Net.Util;
using NuGet.Lucene.Web.Authentication;
using NuGet.Lucene.Web.Formatters;
using NuGet.Lucene.Web.Middleware;

namespace NuGet.Lucene.Web.Extension
{
    public class NuGetMultiRepositoryWebApiModule : Module
    {
        public NuGetMultiRepositoryWebApiModule()
            : this(new NuGetWebApiSettings())
        {
        }

        public NuGetMultiRepositoryWebApiModule(INuGetWebApiSettings nuGetWebApiSettings)
        {
            settings = nuGetWebApiSettings;
        }

        #region Constants

        public const string DefaultRepositoryRoutePrefix = "repository/{repository}/";
        private readonly INuGetWebApiSettings settings;

        #endregion

        #region Public Methods and Operators

        protected virtual IUserStore InitializeUserStore(INuGetWebApiSettings settings)
        {
            var usersDataProvider = InitializeUsersDataProvider(HostingEnvironment.MapPath("~/App_Data/"));
            var userStore = new UserStore(usersDataProvider)
            {
                LocalAdministratorApiKey = settings.LocalAdministratorApiKey,
                HandleLocalRequestsAsAdmin = settings.HandleLocalRequestsAsAdmin
            };
            userStore.Initialize();
            return userStore;
        }


        public virtual LuceneDataProvider InitializeUsersDataProvider(string path)
        {
            var usersIndexPath = Path.Combine(path, "Users");
            var directoryInfo = new DirectoryInfo(usersIndexPath);
            var dir = FSDirectory.Open(directoryInfo, new NativeFSLockFactory(directoryInfo));
            var provider = new LuceneDataProvider(dir, Version.LUCENE_30)
            {
                Settings =
                {
                    EnableMultipleEntities = false,
                    MergeFactor = 2
                }
            };
            return provider;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(settings).As<INuGetWebApiSettings>();

            var routeMapper =
                new NuGetMultiRepositoryWebApiRouteMapper(settings.RoutePathPrefix, DefaultRepositoryRoutePrefix);
            var userStore = InitializeUserStore(settings);

            builder.RegisterInstance(routeMapper);

            builder.RegisterInstance(userStore);

            LoadAuthMiddleware(builder, settings);

            builder.RegisterInstance(new StopSynchronizationCancellationTokenSource());

            builder.RegisterType<PackageFormDataMediaFormatter>().As<MediaTypeFormatter>();
            builder.RegisterApiControllers(typeof(NuGetWebApiModule).Assembly).PropertiesAutowired();
        }

        public virtual void LoadAuthMiddleware(ContainerBuilder builder, INuGetWebApiSettings settings)
        {
            builder.RegisterType<LuceneApiKeyAuthentication>().As<IApiKeyAuthentication>().PropertiesAutowired();

            if (settings.AllowAnonymousPackageChanges)
                builder.RegisterType<AnonymousPackageManagerMiddleware>().InstancePerRequest().PropertiesAutowired();
            else
                builder.RegisterType<ApiKeyAuthenticationMiddleware>().InstancePerRequest().PropertiesAutowired();

            if (settings.HandleLocalRequestsAsAdmin)
                builder.RegisterType<LocalRequestAuthenticationMiddleware>().InstancePerRequest().PropertiesAutowired();

            if (settings.RoleMappingsEnabled)
                builder.RegisterType<RoleMappingAuthenticationMiddleware>().InstancePerRequest().PropertiesAutowired();
        }

        #endregion
    }
}