using Autofac;

namespace nuserv.Service.Contracts
{
    public interface ILifetimeScopeResolver
    {
        #region Public Methods and Operators

        ILifetimeScope Resolve();

        #endregion
    }
}