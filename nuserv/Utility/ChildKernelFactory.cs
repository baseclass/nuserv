using Ninject;
using Ninject.Extensions.ChildKernel;
using Ninject.Selection.Heuristics;
using NuGet.Lucene.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nuserv.Utility
{
    public class ChildKernelFactory : IChildKernelFactory
    {
        public Ninject.Extensions.ChildKernel.IChildKernel Create(Ninject.Syntax.IResolutionRoot resolutionRoot)
        {
            var childKernel = new ChildKernel(resolutionRoot, new NinjectSettings() { LoadExtensions = false });

            childKernel.Components.Add<IInjectionHeuristic, NonDecoratedPropertyInjectionHeuristic>();

            return childKernel;
        }
    }
}