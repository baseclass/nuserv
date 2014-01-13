using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;

namespace nuserv.Utility
{
    public class DependencyScope : IDependencyScope
    {
        private readonly IResolutionRoot resolutionRoot;

        public DependencyScope(IResolutionRoot resolutionRoot)
        {
            this.resolutionRoot = resolutionRoot;
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
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}