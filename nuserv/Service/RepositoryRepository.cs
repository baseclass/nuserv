namespace nuserv.Service
{
    #region Usings

    using System;
    using System.IO;
    using System.Linq;
    using System.Web.Hosting;

    using Lucene.Net.Analysis.Standard;
    using Lucene.Net.Linq;
    using Lucene.Net.Linq.Fluent;
    using Lucene.Net.Linq.Mapping;
    using Lucene.Net.Store;

    using nuserv.Models;
    using nuserv.Service.Contracts;

    using Directory = System.IO.Directory;
    using Version = Lucene.Net.Util.Version;

    #endregion

    public class RepositoryRepository : IRepositoryRepository, IDisposable
    {
        #region Fields

        private readonly IDocumentMapper<Repository> documentMapper;

        private readonly LuceneDataProvider luceneProvider;

        #endregion

        #region Constructors and Destructors

        public RepositoryRepository()
        {
            var directoryPath = GetLucenePath();

            EnsureDirectoryExists(directoryPath);

            var directoryInfo = new DirectoryInfo(directoryPath);

            var directory = FSDirectory.Open(directoryInfo, new NativeFSLockFactory(directoryInfo));

            this.luceneProvider = new LuceneDataProvider(directory, Version.LUCENE_30)
                                  {
                                      Settings =
                                      {
                                          EnableMultipleEntities
                                              = false
                                      }
                                  };

            this.documentMapper = new MapConfiguration().CreateDocumentMapper();
        }

        #endregion

        #region Public Methods and Operators

        public IQueryable<Repository> AsQueryable()
        {
            return this.luceneProvider.AsQueryable(this.documentMapper);
        }

        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        public ISession<Repository> OpenSession()
        {
            return this.luceneProvider.OpenSession(this.documentMapper);
        }

        #endregion

        #region Methods

        private static void EnsureDirectoryExists(string directory)
        {
            if (Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private static string GetLucenePath()
        {
            return HostingEnvironment.MapPath("~/App_Data/repositories");
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // get rid of managed resources
                this.luceneProvider.Dispose();
            }
        }

        #endregion

        private class MapConfiguration
        {
            #region Public Methods and Operators

            public IDocumentMapper<Repository> CreateDocumentMapper()
            {
                var map = new ClassMap<Repository>(Version.LUCENE_30);

                map.Key(r => r.Id);

                map.Property(r => r.Description).AnalyzeWith(new StandardAnalyzer(Version.LUCENE_30));

                map.Property(r => r.Name).AnalyzeWith(new StandardAnalyzer(Version.LUCENE_30));

                return map.ToDocumentMapper();
            }

            #endregion
        }
    }
}