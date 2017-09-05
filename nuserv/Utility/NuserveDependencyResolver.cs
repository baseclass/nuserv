using System.Web.Http.Dependencies;
using nuserv.Service.Contracts;

namespace nuserv.Utility
{
    #region Usings

    #endregion

    public class NuserveDependencyResolver : DependencyScope, IDependencyResolver
    {
        #region Constructors and Destructors

        public NuserveDependencyResolver(ILifetimeScopeResolver lifetimeScopeResolver,
            ILifetimeScopeFactory lifetimeScopeFactory)
            : base(lifetimeScopeResolver.Resolve())
        {
            this.lifetimeScopeResolver = lifetimeScopeResolver;
            this.lifetimeScopeFactory = lifetimeScopeFactory;
        }

        #endregion

        #region Public Methods and Operators

        public IDependencyScope BeginScope()
        {
            var lifetimeScope = lifetimeScopeResolver.Resolve();
            return new DependencyScope(lifetimeScopeFactory.Create(lifetimeScope, builder => { }));
        }

        #endregion

        #region Fields

        private readonly ILifetimeScopeFactory lifetimeScopeFactory;

        private readonly ILifetimeScopeResolver lifetimeScopeResolver;

        #endregion
    }
}