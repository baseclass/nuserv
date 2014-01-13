using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;

namespace nuserv.Utility
{
    public class DependencyResolver : DependencyScope, IDependencyResolver
    {
        private readonly IResolutionRoot resolutionRoot;
        private readonly IChildKernelFactory childKernelFactory;

        public DependencyResolver(IResolutionRoot resolutionRoot, IChildKernelFactory childKernelFactory)
            : base(resolutionRoot)
        {
            this.resolutionRoot = resolutionRoot;
            this.childKernelFactory = childKernelFactory;
        }

        public IDependencyScope BeginScope()
        {
            return new DependencyScope(this.childKernelFactory.Create(this.resolutionRoot));
        }
    }
}