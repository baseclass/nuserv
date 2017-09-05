using System;
using System.Collections.Generic;
using Autofac;
using Lucene.Net.Linq;
using nuserv.Service.Contracts;
using nuserv.Utility;
using NuGet.Lucene;
using NuGet.Lucene.Web;
using NuGet.Lucene.Web.Models;
using NuGet.Lucene.Web.Symbols;

namespace nuserv.Service
{
    #region Usings

    #endregion

    public class RepositoryLifetimeScopeService : IRepositoryLifetimeScopeService
    {
        #region Constants

        public const string AppSettingNamespace = "NuGet.Lucene.Web:";

        #endregion

        #region Constructors and Destructors

        public RepositoryLifetimeScopeService(
            IRepositoryManager repositoryManager,
            ILifetimeScope lifetimeScope,
            ILifetimeScopeFactory lifetimeScopeFactory,
            INuGetWebApiSettings settings,
            StopSynchronizationCancellationTokenSource stopSynchronizationCancellationTokenSource)
        {
            this.repositoryManager = repositoryManager;
            this.lifetimeScope = lifetimeScope;
            this.lifetimeScopeFactory = lifetimeScopeFactory;
            this.settings = settings;
            this.stopSynchronizationCancellationTokenSource = stopSynchronizationCancellationTokenSource;
            repositoryDictionary = new Dictionary<string, ILifetimeScope>();
        }

        #endregion

        #region Fields

        private readonly object lifetimeScopeCreationLock = new object();

        private readonly ILifetimeScopeFactory lifetimeScopeFactory;
        private readonly INuGetWebApiSettings settings;

        private readonly IDictionary<string, ILifetimeScope> repositoryDictionary;

        private readonly IRepositoryManager repositoryManager;

        private readonly ILifetimeScope lifetimeScope;

        private readonly StopSynchronizationCancellationTokenSource stopSynchronizationCancellationTokenSource;

        #endregion

        #region Public Methods and Operators

        public ILifetimeScope GetLifetimeScope(string name)
        {
            if (!RepositoryExists(name))
                throw new ArgumentException(string.Format("Repository {0} not found", name), "name");

            if (!repositoryDictionary.ContainsKey(name))
                lock (lifetimeScopeCreationLock)
                {
                    if (!repositoryDictionary.ContainsKey(name))
                        repositoryDictionary.Add(name, CreateLifetimeScope(name));
                }

            return repositoryDictionary[name];
        }

        public bool RepositoryExists(string name)
        {
            return repositoryManager.Exists(name);
        }

        #endregion

        #region Methods

        private LuceneRepositoryConfigurator CreateLuceneRepositoryConfigurator(string name)
        {
            var cfg = new LuceneRepositoryConfigurator
            {
                EnablePackageFileWatcher = settings.EnablePackageFileWatcher,
                GroupPackageFilesById = settings.GroupPackageFilesById,
                LuceneIndexPath = FormatPath(settings.LucenePackagesIndexPath, name),
                PackagePath = FormatPath(settings.PackagesPath, name),
                PackageOverwriteMode = settings.PackageOverwriteMode,
                LuceneMergeFactor = settings.LuceneMergeFactor,
                DisablePackageHash = settings.DisablePackageHash,
                IgnorePackageFiles = settings.IgnorePackageFiles
            };

            cfg.Initialize();
            return cfg;
        }

        private ILifetimeScope CreateLifetimeScope(string name)
        {
            var lifetime = lifetimeScopeFactory.Create(lifetimeScope, builder =>
            {
                var cfg = CreateLuceneRepositoryConfigurator(name);

                var mirroringPackageRepository = MirroringPackageRepositoryFactory.Create(cfg.Repository,
                    settings.PackageMirrorTargetUrl,
                    settings.PackageMirrorTimeout,
                    settings.AlwaysCheckMirror);

                builder.RegisterInstance(cfg.Repository).OnRelease(repo => cfg.Dispose());
                builder.RegisterInstance(mirroringPackageRepository).As<IMirroringPackageRepository>();
                builder.RegisterInstance(cfg.Provider).As<LuceneDataProvider>();
                var symbolsPath = FormatPath(settings.SymbolsPath, name);
                builder.RegisterInstance(new SymbolTools
                {
                    SymbolPath = symbolsPath,
                    ToolPath = settings.ToolsPath
                }).As<SymbolTools>();
                builder.RegisterInstance(new SymbolSource {SymbolsPath = symbolsPath})
                    .PropertiesAutowired()
                    .As<ISymbolSource>();


                if (settings.SynchronizeOnStart)
                    cfg.Repository.SynchronizeWithFileSystem(SynchronizationMode.Complete,
                        stopSynchronizationCancellationTokenSource.Token);
            });


            return lifetime;
        }

        private string FormatPath(string path, string name)
        {
            if (!path.Contains("{0}"))
                path = path.Replace("~/App_Data/", "~/App_Data/{0}/");

            return string.Format(path, name);
        }

        #endregion
    }
}