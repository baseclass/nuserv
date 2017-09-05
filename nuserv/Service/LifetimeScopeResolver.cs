using System;
using Autofac;
using nuserv.Service.Contracts;
using nuserv.Utility;

namespace nuserv.Service
{
    #region Usings

    #endregion

    public class LifetimeScopeResolver : ILifetimeScopeResolver
    {
        #region Constructors and Destructors

        public LifetimeScopeResolver(IHttpRouteDataResolver routeDataResolver,
            IRepositoryLifetimeScopeService repositoryLifetimeScopeService,
            ILifetimeScope lifetimeScope)
        {
            this.routeDataResolver = routeDataResolver;
            this.lifetimeScope = lifetimeScope;
            this.repositoryLifetimeScopeService = repositoryLifetimeScopeService;
        }

        #endregion

        #region Public Methods and Operators

        public ILifetimeScope Resolve()
        {
            var routeData = routeDataResolver.Resolve();

            if (routeData != null && routeData.Values.ContainsKey("repository"))
            {
                var repositoryName = (string) routeData.Values["repository"];
                if (repositoryLifetimeScopeService.RepositoryExists(repositoryName))
                    return repositoryLifetimeScopeService.GetLifetimeScope(repositoryName);

                throw new InvalidOperationException(string.Format("Repository doesn't exist: {0}", repositoryName));
            }

            return lifetimeScope;
        }

        #endregion

        #region Fields

        private readonly IRepositoryLifetimeScopeService repositoryLifetimeScopeService;

        private readonly ILifetimeScope lifetimeScope;

        private readonly IHttpRouteDataResolver routeDataResolver;

        #endregion
    }
}