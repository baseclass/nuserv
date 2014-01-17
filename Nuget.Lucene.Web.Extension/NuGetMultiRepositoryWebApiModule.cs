namespace NuGet.Lucene.Web.Extension
{
    #region Usings

    using System.Configuration;
    using System.IO;
    using System.Web;
    using System.Web.Hosting;

    using global::Lucene.Net.Linq;
    using global::Lucene.Net.Store;
    using global::Lucene.Net.Util;

    using Ninject.Modules;
    using Ninject.Selection.Heuristics;

    using NuGet.Lucene.Web.Authentication;
    using NuGet.Lucene.Web.Modules;

    #endregion

    public class NuGetMultiRepositoryWebApiModule : NinjectModule
    {
        #region Constants

        public const string AppSettingNamespace = "NuGet.Lucene.Web:";

        public const string DefaultRepositoryRoutePrefix = "repository/{repository}/";

        public const string DefaultRoutePathPrefix = "";

        #endregion

        #region Public Properties

        public static bool AllowAnonymousPackageChanges
        {
            get
            {
                return GetFlagFromAppSetting("allowAnonymousPackageChanges", false);
            }
        }

        public static bool EnableCrossDomainRequests
        {
            get
            {
                return GetFlagFromAppSetting("enableCrossDomainRequests", false);
            }
        }

        public static bool HandleLocalRequestsAsAdmin
        {
            get
            {
                return GetFlagFromAppSetting("handleLocalRequestsAsAdmin", false);
            }
        }

        public static string RepositoryPathPrefix
        {
            get
            {
                return GetAppSetting("repositoryRoutePathPrefix", DefaultRepositoryRoutePrefix);
            }
        }

        public static string RoutePathPrefix
        {
            get
            {
                return GetAppSetting("routePathPrefix", DefaultRoutePathPrefix);
            }
        }

        public static bool ShowExceptionDetails
        {
            get
            {
                return GetFlagFromAppSetting("showExceptionDetails", false);
            }
        }

        #endregion

        #region Public Methods and Operators

        public virtual LuceneDataProvider InitializeUsersDataProvider(string path)
        {
            var usersIndexPath = Path.Combine(path, "Users");
            var directoryInfo = new DirectoryInfo(usersIndexPath);
            var dir = FSDirectory.Open(directoryInfo, new NativeFSLockFactory(directoryInfo));
            var provider = new LuceneDataProvider(dir, Version.LUCENE_30)
                           {
                               Settings =
                               {
                                   EnableMultipleEntities =
                                       false
                               }
                           };
            return provider;
        }

        public override void Load()
        {
            this.Kernel.Components.Add<IInjectionHeuristic, NonDecoratedPropertyInjectionHeuristic>();

            var routeMapper = new NuGetMultiRepositoryWebApiRouteMapper(RoutePathPrefix, RepositoryPathPrefix);
            var usersDataProvider = this.InitializeUsersDataProvider(HostingEnvironment.MapPath("~/App_Data/"));

            this.Bind<NuGetMultiRepositoryWebApiRouteMapper>().ToConstant(routeMapper);

            this.Bind<UserStore>().ToConstant(new UserStore(usersDataProvider));

            this.LoadAuthentication();

            var tokenSource = new ReusableCancellationTokenSource();
            this.Bind<ReusableCancellationTokenSource>().ToConstant(tokenSource);
        }

        public virtual void LoadAuthentication()
        {
            this.Bind<IApiKeyAuthentication>().To<LuceneApiKeyAuthentication>();

            this.Bind<IHttpModule>().To<ApiKeyAuthenticationModule>();

            if (AllowAnonymousPackageChanges)
            {
                this.Bind<IHttpModule>().To<AnonymousPackageManagerModule>();
            }

            if (HandleLocalRequestsAsAdmin)
            {
                this.Bind<IHttpModule>().To<LocalRequestAuthenticationModule>();
            }
        }

        #endregion

        #region Methods

        internal static string GetAppSetting(string key, string defaultValue)
        {
            var value = ConfigurationManager.AppSettings[GetAppSettingKey(key)];
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        internal static bool GetFlagFromAppSetting(string key, bool defaultValue)
        {
            var flag = GetAppSetting(key, string.Empty);

            bool result;
            return bool.TryParse(flag ?? string.Empty, out result) ? result : defaultValue;
        }

        private static string GetAppSettingKey(string key)
        {
            return AppSettingNamespace + key;
        }

        #endregion
    }
}