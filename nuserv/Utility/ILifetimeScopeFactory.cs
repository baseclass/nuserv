using System;
using Autofac;

namespace nuserv.Utility
{
    public interface ILifetimeScopeFactory
    {
        #region Public Methods and Operators

        ILifetimeScope Create(ILifetimeScope lifetimeScope, Action<ContainerBuilder> configurationAction);

        #endregion
    }
}