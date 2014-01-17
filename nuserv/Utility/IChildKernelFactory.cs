namespace nuserv.Utility
{
    #region Usings

    using Ninject.Extensions.ChildKernel;
    using Ninject.Syntax;

    #endregion

    public interface IChildKernelFactory
    {
        #region Public Methods and Operators

        IChildKernel Create(IResolutionRoot resolutionRoot);

        #endregion
    }
}