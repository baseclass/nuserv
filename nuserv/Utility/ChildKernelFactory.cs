namespace nuserv.Utility
{
    #region Usings

    using Ninject;
    using Ninject.Extensions.ChildKernel;
    using Ninject.Selection.Heuristics;

    using NuGet.Lucene.Web;

    #endregion

    public class ChildKernelFactory : IChildKernelFactory
    {
        #region Public Methods and Operators

        public Ninject.Extensions.ChildKernel.IChildKernel Create(Ninject.Syntax.IResolutionRoot resolutionRoot)
        {
            var childKernel = new ChildKernel(resolutionRoot, new NinjectSettings() { LoadExtensions = false });

            childKernel.Components.Add<IInjectionHeuristic, NonDecoratedPropertyInjectionHeuristic>();

            return childKernel;
        }

        #endregion
    }
}