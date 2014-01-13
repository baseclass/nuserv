using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;
using nuserv.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;

namespace nuserv.Utility
{
    public class DependencyResolver : DependencyScope, IDependencyResolver
    {
        private readonly IResolutionRootResolver resolutionRootResolver;
        private readonly IChildKernelFactory childKernelFactory;

        public DependencyResolver(IResolutionRootResolver resolutionRootResolver, IChildKernelFactory childKernelFactory)
            : base(resolutionRootResolver.Resolve())
        {
            this.resolutionRootResolver = resolutionRootResolver;
            this.childKernelFactory = childKernelFactory;
        }

        public IDependencyScope BeginScope()
        {
            var resolutionRoot = this.resolutionRootResolver.Resolve();
            return new DependencyScope(this.childKernelFactory.Create(resolutionRoot));
        }
    }
}