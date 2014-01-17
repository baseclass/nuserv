namespace nuserv.Utility
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http.Dependencies;

    using Ninject;
    using Ninject.Parameters;
    using Ninject.Syntax;

    #endregion

    public class DependencyScope : IDependencyScope
    {
        #region Fields

        private readonly IResolutionRoot resolutionRoot;

        #endregion

        #region Constructors and Destructors

        public DependencyScope(IResolutionRoot resolutionRoot)
        {
            this.resolutionRoot = resolutionRoot;
        }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            var disposable = this.resolutionRoot as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
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

        #endregion
    }
}