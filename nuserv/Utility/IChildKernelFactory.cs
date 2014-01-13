using Ninject.Extensions.ChildKernel;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nuserv.Utility
{
    public interface IChildKernelFactory
    {
        IChildKernel Create(IResolutionRoot resolutionRoot);
    }
}
