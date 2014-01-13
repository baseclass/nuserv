using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Hosting;
using Lucene.Net.Linq;
using Lucene.Net.Store;
using Ninject;
using Ninject.Components;
using Ninject.Modules;
using Ninject.Selection.Heuristics;
using NuGet.Lucene;
using NuGet.Lucene.Web;
using NuGet.Lucene.Web.Authentication;
using NuGet.Lucene.Web.Models;
using NuGet.Lucene.Web.Modules;
using Version = Lucene.Net.Util.Version;
using System.Web.Http.WebHost;
using System.Web.Http.WebHost.Routing;
using System.Net.Http;

namespace NuGet.Lucene.Web.Extension
{
   public class NuGetMultiRepositoryWebApiModule : NinjectModule
   {
        public const string AppSettingNamespace = "NuGet.Lucene.Web:";
        public const string DefaultRoutePathPrefix = "api/";
        public const string DefaultRepositoryRoutePrefix = "repository/{repository}/";

        public override void Load()
        {
            Kernel.Components.Add<IInjectionHeuristic, NonDecoratedPropertyInjectionHeuristic>();

            var routeMapper = new NuGetMultiRepositoryWebApiRouteMapper(RoutePathPrefix, RepositoryPathPrefix);
            var usersDataProvider = InitializeUsersDataProvider(HostingEnvironment.MapPath("~/App_Data/"));

            Bind<NuGetMultiRepositoryWebApiRouteMapper>().ToConstant(routeMapper);

            Bind<UserStore>().ToConstant(new UserStore(usersDataProvider));
            
            LoadAuthentication();

            var tokenSource = new ReusableCancellationTokenSource();
            Bind<ReusableCancellationTokenSource>().ToConstant(tokenSource);
        }

        public virtual void LoadAuthentication()
        {
            Bind<IApiKeyAuthentication>().To<LuceneApiKeyAuthentication>();

            Bind<IHttpModule>().To<ApiKeyAuthenticationModule>();

            if (AllowAnonymousPackageChanges)
            {
                Bind<IHttpModule>().To<AnonymousPackageManagerModule>();
            }

            if (HandleLocalRequestsAsAdmin)
            {
                Bind<IHttpModule>().To<LocalRequestAuthenticationModule>();
            }
        }

        public virtual LuceneDataProvider InitializeUsersDataProvider(string path)
        {
            var usersIndexPath = Path.Combine(path, "Users");
            var directoryInfo = new DirectoryInfo(usersIndexPath);
            var dir = FSDirectory.Open(directoryInfo, new NativeFSLockFactory(directoryInfo));
            var provider = new LuceneDataProvider(dir, Version.LUCENE_30);
            provider.Settings.EnableMultipleEntities = false;
            return provider;
        }

        public static bool ShowExceptionDetails
        {
            get { return GetFlagFromAppSetting("showExceptionDetails", false); }
        }
        
        public static bool EnableCrossDomainRequests
        {
            get { return GetFlagFromAppSetting("enableCrossDomainRequests", false); }
        }

        public static bool HandleLocalRequestsAsAdmin
        {
            get { return GetFlagFromAppSetting("handleLocalRequestsAsAdmin", false); }
        }

        public static bool AllowAnonymousPackageChanges
        {
            get { return GetFlagFromAppSetting("allowAnonymousPackageChanges", false); }
        }

        public static string RoutePathPrefix
        {
            get { return GetAppSetting("routePathPrefix", DefaultRoutePathPrefix); }
        }

        public static string RepositoryPathPrefix
        {
            get { return GetAppSetting("repositoryRoutePathPrefix", DefaultRepositoryRoutePrefix); }
        }

        internal static bool GetFlagFromAppSetting(string key, bool defaultValue)
        {
            var flag = GetAppSetting(key, string.Empty);

            bool result;
            return bool.TryParse(flag ?? string.Empty, out result) ? result : defaultValue;
        }

        internal static string GetAppSetting(string key, string defaultValue)
        {
            var value = ConfigurationManager.AppSettings[GetAppSettingKey(key)];
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        private static string GetAppSettingKey(string key)
        {
            return AppSettingNamespace + key;
        }
    }
}
