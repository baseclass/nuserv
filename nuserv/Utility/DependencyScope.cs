using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Autofac;

namespace nuserv.Utility
{
    public class DependencyScope : IDependencyScope
    {
        #region Fields

        private readonly ILifetimeScope lifetimeScope;

        #endregion

        #region Constructors and Destructors

        public DependencyScope(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            var disposable = lifetimeScope as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }

        public object GetService(Type serviceType)
        {
            if (!lifetimeScope.IsRegistered(serviceType))
            {
                return null;
            }

            return lifetimeScope.Resolve(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return (IEnumerable<object>) lifetimeScope.Resolve(typeof(IEnumerable<>).MakeGenericType(serviceType));
        }

        #endregion
    }
}