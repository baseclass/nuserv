[assembly: WebActivator.PreApplicationStartMethod(typeof(nuserv.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(nuserv.App_Start.NinjectWebCommon), "Stop")]

namespace nuserv.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using NuGet.Lucene.Web;
    using System.Web.Routing;
    using System.Web.Http;
    using AspNet.WebApi.HtmlMicrodataFormatter;
    using System.Web.Http.Description;
    using NuGet.Lucene.Web.Formatters;
    using System.Web.Mvc;
    using System.Reflection;
    using Ninject.Web.WebApi;
    using NuGet.Lucene.Web.Extension;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel(new NuGetMultiRepositoryWebApiModule());
            
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            kernel.Unbind<System.Web.Http.Dependencies.IDependencyResolver>();
            kernel.Bind<System.Web.Http.Dependencies.IDependencyResolver>().To<Utility.DependencyResolver>();

            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            var routeMapper = kernel.Get<NuGetMultiRepositoryWebApiRouteMapper>();
            routeMapper.MapApiRoutes(GlobalConfiguration.Configuration);
            
            //Currently not working!
            //routeMapper.MapDataServiceRoutes(RouteTable.Routes);

            var config = GlobalConfiguration.Configuration;

            config.Formatters.Add(new PackageFormDataMediaFormatter());

            // load xml documentation for assemblies
            var documentation = new HtmlDocumentation();
            documentation.Load();
            config.Services.Replace(typeof(IDocumentationProvider), new WebApiHtmlDocumentationProvider(documentation));

            // register the formatter
            config.Formatters.Add(new NuGetHtmlMicrodataFormatter());
        }        
    }
}
