using System;
using Autofac;

namespace nuserv.Utility
{
    public class LifetimeScopeFactory : ILifetimeScopeFactory
    {
        #region Public Methods and Operators

        public ILifetimeScope Create(ILifetimeScope lifetimeScope, Action<ContainerBuilder> configurationAction)
        {
            return lifetimeScope.BeginLifetimeScope(configurationAction);
        }

        #endregion
    }
}