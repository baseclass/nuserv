using System.Linq;
using System.Threading.Tasks;
using Autofac;
using nuserv.Service.Contracts;
using NuGet;
using NuGet.Lucene;
using NuGet.Lucene.Web.Formatters;

namespace nuserv.Service
{
    public class MultiRepositoryPackageFormDataMediaFormatter : MultipartFormDataMediaFormatter<IPackage,
        HashingMultipartFileStreamProvider>
    {
        private readonly ILifetimeScopeResolver lifetimeScopeResolver;

        public MultiRepositoryPackageFormDataMediaFormatter(ILifetimeScopeResolver lifetimeScopeResolver)
        {
            this.lifetimeScopeResolver = lifetimeScopeResolver;
        }

        private ILucenePackageRepository Repository => lifetimeScopeResolver.Resolve()
            .Resolve<ILucenePackageRepository>();

        public override HashingMultipartFileStreamProvider CreateStreamProvider()
        {
            return new HashingMultipartFileStreamProvider(Repository);
        }

        public override Task<IPackage> ReadFormDataFromStreamAsync(HashingMultipartFileStreamProvider streamProvider)
        {
            IFastZipPackage package = null;

            try
            {
                var packageStream = streamProvider.ContentStreams.Single();

                package = Repository.LoadStagedPackage(packageStream);
            }
            finally
            {
                if (package == null)
                    foreach (var stream in streamProvider.ContentStreams)
                        Repository.DiscardStagedPackage(stream);
            }

            return Task.FromResult<IPackage>(package);
        }
    }
}