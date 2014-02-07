namespace nuserv
{
    #region Usings

    using System;
    using System.Web.Http;
    using System.Web.Http.Dependencies;
    using System.Web.Routing;

    using Ninject;
    using Ninject.Extensions.Factory;
    using Ninject.Modules;
    using Ninject.Parameters;
    using Ninject.Syntax;
    using Ninject.Web.Common;

    using NuGet.Lucene.Web.Extension;

    using nuserv.Models;
    using nuserv.Models.Contracts;
    using nuserv.Service;
    using nuserv.Service.Contracts;
    using nuserv.Utility;

    using IInstanceProvider = System.ServiceModel.Dispatcher.IInstanceProvider;

    #endregion

    public class NuservModule : NinjectModule
    {
        #region Public Methods and Operators

        public override void Load()
        {
            RegisterServices(this.Kernel);

            Initialize(this.Kernel);
        }

        #endregion

        #region Methods

        private static void Initialize(IResolutionRoot resolutionRoot)
        {
            var repositoryKernelService = resolutionRoot.Get<IRepositoryKernelService>();
            repositoryKernelService.Init();

            var repositoryManager = resolutionRoot.Get<IRepositoryManager>();
            repositoryManager.Init();

            var routeMapper = resolutionRoot.Get<NuGetMultiRepositoryWebApiRouteMapper>();

            routeMapper.MapApiRoutes(GlobalConfiguration.Configuration);
            routeMapper.MapDataServiceRoutes(RouteTable.Routes);
        }

        private static void RegisterServices(IBindingRoot bindingRoot)
        {
            bindingRoot.Bind<IChildKernelFactory>().To<ChildKernelFactory>().InSingletonScope();
            bindingRoot.Bind<IResolutionRootResolver>().To<ResolutionRootResolver>().InRequestScope();
            bindingRoot.Bind<IRepositoryKernelService>().To<RepositoryKernelService>().InSingletonScope();
            bindingRoot.Bind<IHttpRouteDataResolver>().To<HttpRouteDataResolver>().InSingletonScope();
            bindingRoot.Bind<IRepositoryManager>().To<RepositoryManager>().InSingletonScope();

            bindingRoot.Bind<IRepository>().To<Repository>();
            bindingRoot.Bind<IRepositoryFactory>().ToFactory().InSingletonScope();

            // Remove default ninject dependency resolver and bind to one which creates a child kernel per request
            // and uses an IResolutionRootResolver to get the resolution root to create a child kernel from.
            bindingRoot.Unbind<IDependencyResolver>();
            bindingRoot.Bind<IDependencyResolver>().To<DependencyResolver>();

            // Remove default ninject IInstanceProvider and use one which uses an IDependencyResolver to create an instance.
            // That way WCF uses the same mechanism as WebAPI to get dependencies.
            bindingRoot.Bind<IInstanceProvider>().To<DependencyResolverInstanceProvider>();
            bindingRoot.Unbind<Func<Type, IInstanceProvider>>();
            bindingRoot.Bind<Func<Type, IInstanceProvider>>()
                .ToMethod(
                    ctx => type => ctx.Kernel.Get<IInstanceProvider>(new ConstructorArgument("serviceType", type)));
        }

        #endregion
    }
}