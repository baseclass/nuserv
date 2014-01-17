namespace nuserv.Service
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web.Hosting;

    using Lucene.Net.Linq;

    using Ninject.Syntax;

    using NuGet.Lucene;
    using NuGet.Lucene.Web;
    using NuGet.Lucene.Web.Models;

    using nuserv.Utility;

    #endregion

    public class RepositoryKernelService : IRepositoryKernelService
    {
        #region Constants

        public const string AppSettingNamespace = "NuGet.Lucene.Web:";

        #endregion

        #region Fields

        private readonly IChildKernelFactory childKernelFactory;

        private readonly IDictionary<string, IResolutionRoot> repositoryDictionary;

        private readonly IResolutionRoot resolutionRoot;

        #endregion

        #region Constructors and Destructors

        public RepositoryKernelService(IResolutionRoot resolutionRoot, IChildKernelFactory childKernelFactory)
        {
            this.resolutionRoot = resolutionRoot;
            this.childKernelFactory = childKernelFactory;
            this.repositoryDictionary = new Dictionary<string, IResolutionRoot>();
        }

        #endregion

        #region Public Properties

        public static string PackageMirrorTargetUrl
        {
            get
            {
                return GetAppSetting("packageMirrorTargetUrl", string.Empty);
            }
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

        #endregion

        #region Properties

        [Ninject.Inject]
        private ReusableCancellationTokenSource TokenSource { get; set; }

        #endregion

        #region Public Methods and Operators

        public IResolutionRoot GetChildKernel(string name)
        {
            return this.repositoryDictionary[name];
        }

        public void Init()
        {
            var cfg = new LuceneRepositoryConfigurator
                      {
                          EnablePackageFileWatcher =
                              GetFlagFromAppSetting("enablePackageFileWatcher", true),
                          GroupPackageFilesById =
                              GetFlagFromAppSetting("groupPackageFilesById", true),
                          LuceneIndexPath =
                              MapPathFromAppSetting(
                                  "lucenePath",
                                  "~/App_Data/repo1/Lucene"),
                          PackagePath =
                              MapPathFromAppSetting(
                                  "packagesPath",
                                  "~/App_Data/repo1/Packages")
                      };

            var cfg2 = new LuceneRepositoryConfigurator
                       {
                           EnablePackageFileWatcher =
                               GetFlagFromAppSetting("enablePackageFileWatcher", true),
                           GroupPackageFilesById =
                               GetFlagFromAppSetting("groupPackageFilesById", true),
                           LuceneIndexPath =
                               MapPathFromAppSetting(
                                   "lucenePath",
                                   "~/App_Data/repo2/Lucene"),
                           PackagePath =
                               MapPathFromAppSetting(
                                   "packagesPath",
                                   "~/App_Data/repo2/Packages")
                       };

            cfg.Initialize();

            cfg2.Initialize();

            var mirroringPackageRepository = MirroringPackageRepositoryFactory.Create(
                cfg.Repository,
                PackageMirrorTargetUrl,
                PackageMirrorTimeout);
            var mirroringPackageRepository2 = MirroringPackageRepositoryFactory.Create(
                cfg2.Repository,
                PackageMirrorTargetUrl,
                PackageMirrorTimeout);

            var repo1Kernel = this.childKernelFactory.Create(this.resolutionRoot);

            repo1Kernel.Bind<ILucenePackageRepository>().ToConstant(cfg.Repository).OnDeactivation(_ => cfg.Dispose());
            repo1Kernel.Bind<IMirroringPackageRepository>().ToConstant(mirroringPackageRepository);
            repo1Kernel.Bind<LuceneDataProvider>().ToConstant(cfg.Provider);

            var repo2Kernel = this.childKernelFactory.Create(this.resolutionRoot);

            repo2Kernel.Bind<ILucenePackageRepository>().ToConstant(cfg2.Repository).OnDeactivation(_ => cfg.Dispose());
            repo2Kernel.Bind<IMirroringPackageRepository>().ToConstant(mirroringPackageRepository2);
            repo2Kernel.Bind<LuceneDataProvider>().ToConstant(cfg2.Provider);

            this.repositoryDictionary.Add("repo1", repo1Kernel);
            this.repositoryDictionary.Add("repo2", repo2Kernel);

            if (GetFlagFromAppSetting("synchronizeOnStart", true))
            {
                cfg.Repository.SynchronizeWithFileSystem(this.TokenSource.Token);
                cfg2.Repository.SynchronizeWithFileSystem(this.TokenSource.Token);
            }
        }

        public bool RepositoryExists(string name)
        {
            return name == "repo1" || name == "repo2";
        }

        #endregion

        #region Methods

        private static string GetAppSetting(string key, string defaultValue)
        {
            var value = ConfigurationManager.AppSettings[GetAppSettingKey(key)];
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        private static string GetAppSettingKey(string key)
        {
            return AppSettingNamespace + key;
        }

        private static bool GetFlagFromAppSetting(string key, bool defaultValue)
        {
            var flag = GetAppSetting(key, string.Empty);

            bool result;
            return bool.TryParse(flag ?? string.Empty, out result) ? result : defaultValue;
        }

        private static string MapPathFromAppSetting(string key, string defaultValue)
        {
            var path = GetAppSetting(key, defaultValue);

            if (path.StartsWith("~/"))
            {
                return HostingEnvironment.MapPath(path);
            }

            return path;
        }

        #endregion
    }
}