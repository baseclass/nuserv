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
        public const string AppSettingNamesapce = "NuGet.Lucene.Web:";
        public const string DefaultRoutePathPrefix = "api/";
        public const string DefaultRepositoryRoutePrefix = "repository/{repository}/";

        public override void Load()
        {
            var cfg = new LuceneRepositoryConfigurator
                {
                    EnablePackageFileWatcher = GetFlagFromAppSetting("enablePackageFileWatcher", true),
                    GroupPackageFilesById = GetFlagFromAppSetting("groupPackageFilesById", true),
                    LuceneIndexPath = MapPathFromAppSetting("lucenePath", "~/App_Data/repo1/Lucene"),
                    PackagePath = MapPathFromAppSetting("packagesPath", "~/App_Data/repo1/Packages")
                };

            var cfg2 = new LuceneRepositoryConfigurator
            {
                EnablePackageFileWatcher = GetFlagFromAppSetting("enablePackageFileWatcher", true),
                GroupPackageFilesById = GetFlagFromAppSetting("groupPackageFilesById", true),
                LuceneIndexPath = MapPathFromAppSetting("lucenePath", "~/App_Data/repo2/Lucene"),
                PackagePath = MapPathFromAppSetting("packagesPath", "~/App_Data/repo2/Packages")
            };

            cfg.Initialize();

            cfg2.Initialize();

            Kernel.Components.Add<IInjectionHeuristic, NonDecoratedPropertyInjectionHeuristic>();

            var routeMapper = new NuGetMultiRepositoryWebApiRouteMapper(RoutePathPrefix, RepositoryPathPrefix);
            var mirroringPackageRepository = MirroringPackageRepositoryFactory.Create(cfg.Repository, PackageMirrorTargetUrl, PackageMirrorTimeout);
            var mirroringPackageRepository2 = MirroringPackageRepositoryFactory.Create(cfg2.Repository, PackageMirrorTargetUrl, PackageMirrorTimeout);
            var usersDataProvider = InitializeUsersDataProvider("~/App_Data/");

            Bind<NuGetMultiRepositoryWebApiRouteMapper>().ToConstant(routeMapper);

            Bind<ILucenePackageRepository>().ToConstant(cfg.Repository).When(req => IsRepo("repo1")).OnDeactivation(_ => cfg.Dispose());
            Bind<IMirroringPackageRepository>().ToConstant(mirroringPackageRepository).When(req => IsRepo("repo1"));
            Bind<LuceneDataProvider>().ToConstant(cfg.Provider).When(req => IsRepo("repo1"));
            Bind<UserStore>().ToConstant(new UserStore(usersDataProvider));//.When(req => IsRepo("repo1"));

            Bind<ILucenePackageRepository>().ToConstant(cfg2.Repository).When(req => IsRepo("repo2")).OnDeactivation(_ => cfg.Dispose());
            Bind<IMirroringPackageRepository>().ToConstant(mirroringPackageRepository2).When(req => IsRepo("repo2"));
            Bind<LuceneDataProvider>().ToConstant(cfg2.Provider).When(req => IsRepo("repo2"));
            
            LoadAuthentication();

            var tokenSource = new ReusableCancellationTokenSource();
            Bind<ReusableCancellationTokenSource>().ToConstant(tokenSource);

            if (GetFlagFromAppSetting("synchronizeOnStart", true))
            {
                cfg.Repository.SynchronizeWithFileSystem(tokenSource.Token);
                cfg2.Repository.SynchronizeWithFileSystem(tokenSource.Token);  
            }
        }

        private static bool IsRepo(string repository)
        {
            var mvcHandler = (HttpControllerHandler)HttpContext.Current.Handler;

            var requestMessage = HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage;

            var routedata = requestMessage.GetRouteData();
            var routeValues = routedata.Values;

            var containsKey = routeValues.ContainsKey("repository");

            if (!containsKey)
            {
                return false;
            }

            return (string)routeValues["repository"] == repository;
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

        public static string PackageMirrorTargetUrl
        {
            get { return GetAppSetting("packageMirrorTargetUrl", string.Empty); }
        }

        public static TimeSpan PackageMirrorTimeout
        {
            get
            {
                var str = GetAppSetting("packageMirrorTimeout", "0:00:15");
                TimeSpan ts;
                return TimeSpan.TryParse(str, out ts) ? ts : TimeSpan.FromSeconds(15);
            }
        }
        internal static bool GetFlagFromAppSetting(string key, bool defaultValue)
        {
            var flag = GetAppSetting(key, string.Empty);

            bool result;
            return bool.TryParse(flag ?? string.Empty, out result) ? result : defaultValue;
        }
        
        internal static string MapPathFromAppSetting(string key, string defaultValue)
        {
            var path = GetAppSetting(key, defaultValue);

            if (path.StartsWith("~/"))
            {
                return HostingEnvironment.MapPath(path);
            }

            return path;
        }

        internal static string GetAppSetting(string key, string defaultValue)
        {
            var value = ConfigurationManager.AppSettings[GetAppSettingKey(key)];
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        private static string GetAppSettingKey(string key)
        {
            return AppSettingNamesapce + key;
        }
    }
}
