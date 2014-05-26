namespace nuserv.Service
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web.Hosting;

    using Lucene.Net.Linq;

    using Ninject.Extensions.ChildKernel;
    using Ninject.Syntax;

    using NuGet.Lucene;
    using NuGet.Lucene.Web;
    using NuGet.Lucene.Web.Models;

    using nuserv.Service.Contracts;
    using nuserv.Utility;

    #endregion

    public class RepositoryKernelService : IRepositoryKernelService
    {
        #region Constants

        public const string AppSettingNamespace = "NuGet.Lucene.Web:";

        #endregion

        #region Fields

        private readonly object childKernelCreationLock = new object();

        private readonly IChildKernelFactory childKernelFactory;

        private readonly IDictionary<string, IResolutionRoot> repositoryDictionary;

        private readonly IRepositoryManager repositoryManager;

        private readonly IResolutionRoot resolutionRoot;

        private readonly ReusableCancellationTokenSource reusableCancellationTokenSource;

        #endregion

        #region Constructors and Destructors

        public RepositoryKernelService(
            IRepositoryManager repositoryManager,
            IResolutionRoot resolutionRoot,
            IChildKernelFactory childKernelFactory,
            ReusableCancellationTokenSource reusableCancellationTokenSource)
        {
            this.repositoryManager = repositoryManager;
            this.resolutionRoot = resolutionRoot;
            this.childKernelFactory = childKernelFactory;
            this.reusableCancellationTokenSource = reusableCancellationTokenSource;
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

        #region Public Methods and Operators

        public IResolutionRoot GetChildKernel(string name)
        {
            if (!this.RepositoryExists(name))
            {
                throw new ArgumentException(string.Format("Repository {0} not found", name), "name");
            }

            if (!this.repositoryDictionary.ContainsKey(name))
            {
                lock (this.childKernelCreationLock)
                {
                    if (!this.repositoryDictionary.ContainsKey(name))
                    {
                        this.repositoryDictionary.Add(name, this.CreateKernel(name));
                    }
                }
            }

            return this.repositoryDictionary[name];
        }

        public bool RepositoryExists(string name)
        {
            return this.repositoryManager.Exists(name);
        }

        #endregion

        #region Methods

        private static LuceneRepositoryConfigurator CreateLuceneRepositoryConfigurator(string name)
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
                                  string.Format("~/App_Data/{0}/Lucene", name)),
                          PackagePath =
                              MapPathFromAppSetting(
                                  "packagesPath",
                                  string.Format("~/App_Data/{0}/Packages", name))
                      };

            cfg.Initialize();
            return cfg;
        }

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

        private IChildKernel CreateKernel(string name)
        {
            var cfg = CreateLuceneRepositoryConfigurator(name);

            var mirroringPackageRepository = MirroringPackageRepositoryFactory.Create(cfg.Repository,
                PackageMirrorTargetUrl,
                PackageMirrorTimeout, false);

            var kernel = this.childKernelFactory.Create(this.resolutionRoot);

            kernel.Bind<ILucenePackageRepository>().ToConstant(cfg.Repository).OnDeactivation(_ => cfg.Dispose());
            kernel.Bind<IMirroringPackageRepository>().ToConstant(mirroringPackageRepository);
            kernel.Bind<LuceneDataProvider>().ToConstant(cfg.Provider);

            if (GetFlagFromAppSetting("synchronizeOnStart", true))
            {
                cfg.Repository.SynchronizeWithFileSystem(this.reusableCancellationTokenSource.Token);
            }
            return kernel;
        }

        #endregion
    }
}