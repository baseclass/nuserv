using Ninject;
using Ninject.Extensions.ChildKernel;
using Ninject.Parameters;
using Ninject.Selection.Heuristics;
using Ninject.Syntax;
using Ninject.Web.WebApi;
using NuGet.Lucene.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;

namespace nuserv.Utility
{
    public class DependencyResolver : IDependencyResolver
    {
        private readonly IResolutionRoot resolutionRoot;

        public DependencyResolver(IResolutionRoot resolutionRoot)
        {
            this.resolutionRoot = resolutionRoot;
        }

        public IDependencyScope BeginScope()
        {
            //return this;

            var childKernel = new ChildKernel(this.resolutionRoot, new NinjectSettings() { LoadExtensions = false });
            childKernel.Components.Add<IInjectionHeuristic, NonDecoratedPropertyInjectionHeuristic>();

            return new DependencyResolver(childKernel);
        }

        public object GetService(Type serviceType)
        {
            var request = this.resolutionRoot.CreateRequest(serviceType, null, new Parameter[0], true, true);
            return this.resolutionRoot.Resolve(request).SingleOrDefault();
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.resolutionRoot.GetAll(serviceType);
        }

        public void Dispose()
        {
            var disposable = this.resolutionRoot as IDisposable;
            if(disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}