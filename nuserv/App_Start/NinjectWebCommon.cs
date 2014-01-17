[assembly: WebActivator.PreApplicationStartMethod(typeof(nuserv.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(nuserv.App_Start.NinjectWebCommon), "Stop")]

// ReSharper disable once CheckNamespace

namespace nuserv.App_Start
{
    #region Usings

    using System;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Dependencies;
    using System.Web.Routing;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Parameters;
    using Ninject.Web.Common;

    using NuGet.Lucene.Web.DataServices;
    using NuGet.Lucene.Web.Extension;

    using nuserv.Service;
    using nuserv.Utility;

    #endregion

    public static class NinjectWebCommon
    {
        #region Static Fields

        private static readonly Bootstrapper Bootstrapper = new Bootstrapper();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            Bootstrapper.Initialize(CreateKernel);

            var repositoryKernelService = Bootstrapper.Kernel.Get<IRepositoryKernelService>();
            repositoryKernelService.Init();
        }

        /// <summary>
        ///     Stops the application.
        /// </summary>
        public static void Stop()
        {
            Bootstrapper.ShutDown();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel(new NuGetMultiRepositoryWebApiModule());

            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            kernel.Unbind<IDependencyResolver>();
            kernel.Bind<IDependencyResolver>().To<DependencyResolver>();

            kernel.Bind<IInstanceProvider>().To<DependencyResolverInstanceProvider>();
            kernel.Unbind<Func<Type, IInstanceProvider>>();
            kernel.Bind<Func<Type, IInstanceProvider>>().ToMethod(ctx => type => ctx.Kernel.Get<IInstanceProvider>(new ConstructorArgument("serviceType", type)));

            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        ///     Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            var routeMapper = kernel.Get<NuGetMultiRepositoryWebApiRouteMapper>();
            routeMapper.MapApiRoutes(GlobalConfiguration.Configuration);

            routeMapper.MapDataServiceRoutes(RouteTable.Routes);

            kernel.Bind<IChildKernelFactory>().To<ChildKernelFactory>().InSingletonScope();
            kernel.Bind<IResolutionRootResolver>().To<ResolutionRootResolver>().InRequestScope();
            kernel.Bind<IRepositoryKernelService>().To<RepositoryKernelService>().InSingletonScope();
            kernel.Bind<IHttpRouteDataResolver>().To<HttpRouteDataResolver>().InSingletonScope();
        }

        #endregion
    }
}