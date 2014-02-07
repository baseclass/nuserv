namespace nuserv.Utility
{
    #region Usings

    using System.Web.Http.Dependencies;

    using nuserv.Service.Contracts;

    #endregion

    public class DependencyResolver : DependencyScope, IDependencyResolver
    {
        #region Fields

        private readonly IChildKernelFactory childKernelFactory;

        private readonly IResolutionRootResolver resolutionRootResolver;

        #endregion

        #region Constructors and Destructors

        public DependencyResolver(
            IResolutionRootResolver resolutionRootResolver,
            IChildKernelFactory childKernelFactory)
            : base(resolutionRootResolver.Resolve())
        {
            this.resolutionRootResolver = resolutionRootResolver;
            this.childKernelFactory = childKernelFactory;
        }

        #endregion

        #region Public Methods and Operators

        public IDependencyScope BeginScope()
        {
            var resolutionRoot = this.resolutionRootResolver.Resolve();
            return new DependencyScope(this.childKernelFactory.Create(resolutionRoot));
        }

        #endregion
    }
}