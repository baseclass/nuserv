namespace nuserv.Service.Contracts
{
    #region Usings

    using Ninject.Syntax;

    #endregion

    public interface IRepositoryKernelService
    {
        #region Public Methods and Operators

        IResolutionRoot GetChildKernel(string name);

        bool RepositoryExists(string name);

        #endregion
    }
}