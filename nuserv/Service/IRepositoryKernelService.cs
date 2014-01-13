using Ninject.Extensions.ChildKernel;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nuserv.Service
{
    public interface IRepositoryKernelService
    {
        void Init();

        bool RepositoryExists(string name);

        IResolutionRoot GetChildKernel(string name);
    }
}
