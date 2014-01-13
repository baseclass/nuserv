using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace nuserv.Service
{
    public class ResolutionRootResolver : IResolutionRootResolver
    {
        private readonly HttpRequestMessage requestMessage;

        private readonly IRepositoryKernelService repositoryKernelService;

        private readonly IResolutionRoot resolutionRoot;

        public ResolutionRootResolver(HttpRequestMessage requestMessage, 
                                      IRepositoryKernelService repositoryKernelService, 
                                      IResolutionRoot resolutionRoot)
        {
            this.requestMessage = requestMessage;
            this.resolutionRoot = resolutionRoot;
            this.repositoryKernelService = repositoryKernelService;
        }

        public Ninject.Syntax.IResolutionRoot Resolve()
        {
            //var routeData = this.requestMessage.GetRouteData();

            if (HttpContext.Current != null)
            {
                var requestMessage = HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage;

                var routeData = requestMessage.GetRouteData();

                if (routeData != null && routeData.Values.ContainsKey("repository"))
                {
                    var repositoryName = (string)routeData.Values["repository"];
                    if (this.repositoryKernelService.RepositoryExists(repositoryName))
                    {
                        return this.repositoryKernelService.GetChildKernel(repositoryName);
                    }

                    throw new InvalidOperationException(string.Format("Repository doesn't exist: {0}", repositoryName));
                }
            }

            return resolutionRoot;
        }
    }
}