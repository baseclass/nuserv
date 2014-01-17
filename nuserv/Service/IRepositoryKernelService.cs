namespace nuserv.Service
{
    #region Usings

    using Ninject.Syntax;

    #endregion

    public interface IRepositoryKernelService
    {
        #region Public Methods and Operators

        IResolutionRoot GetChildKernel(string name);

        void Init();

        bool RepositoryExists(string name);

        #endregion
    }
}