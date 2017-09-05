using Autofac;

namespace nuserv.Service.Contracts
{
    public interface IRepositoryLifetimeScopeService
    {
        #region Public Methods and Operators

        ILifetimeScope GetLifetimeScope(string name);

        bool RepositoryExists(string name);

        #endregion
    }
}