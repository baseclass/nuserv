namespace nuserv.Service
{
    #region Usings

    using System;

    using Ninject.Syntax;

    using nuserv.Service.Contracts;
    using nuserv.Utility;

    #endregion

    public class ResolutionRootResolver : IResolutionRootResolver
    {
        #region Fields

        private readonly IRepositoryKernelService repositoryKernelService;

        private readonly IResolutionRoot resolutionRoot;

        private readonly IHttpRouteDataResolver routeDataResolver;

        #endregion

        #region Constructors and Destructors

        public ResolutionRootResolver(
            IHttpRouteDataResolver routeDataResolver,
            IRepositoryKernelService repositoryKernelService,
            IResolutionRoot resolutionRoot)
        {
            this.routeDataResolver = routeDataResolver;
            this.resolutionRoot = resolutionRoot;
            this.repositoryKernelService = repositoryKernelService;
        }

        #endregion

        #region Public Methods and Operators

        public IResolutionRoot Resolve()
        {
            var routeData = this.routeDataResolver.Resolve();

            if (routeData != null && routeData.Values.ContainsKey("repository"))
            {
                var repositoryName = (string)routeData.Values["repository"];
                if (this.repositoryKernelService.RepositoryExists(repositoryName))
                {
                    return this.repositoryKernelService.GetChildKernel(repositoryName);
                }

                throw new InvalidOperationException(string.Format("Repository doesn't exist: {0}", repositoryName));
            }

            return this.resolutionRoot;
        }

        #endregion
    }
}