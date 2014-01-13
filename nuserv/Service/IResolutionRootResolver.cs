using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nuserv.Service
{
    public interface IResolutionRootResolver
    {
        IResolutionRoot Resolve();
    }
}