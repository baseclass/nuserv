#region Usings

using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ExceptionHandling;
using System.Web.Mvc;
using AspNet.WebApi.HtmlMicrodataFormatter;
using Autofac;
using Common.Logging;
using Microsoft.Owin;
using Microsoft.Owin.Diagnostics;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Infrastructure;
using nuserv.Service;
using nuserv.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NuGet;
using NuGet.Lucene;
using NuGet.Lucene.Web;
using NuGet.Lucene.Web.Extension;
using NuGet.Lucene.Web.Filters;
using NuGet.Lucene.Web.Formatters;
using NuGet.Lucene.Web.MessageHandlers;
using NuGet.Lucene.Web.Util;
using Owin;
using Startup = nuserv.Startup;

#endregion

[assembly: OwinStartup(typeof(Startup))]

namespace nuserv
{
    public class Startup
    {
        private static readonly ILog Log = LogManager.GetLogger<Startup>();

        protected readonly ManualResetEventSlim shutdownSignal = new ManualResetEventSlim(false);

        public INuGetWebApiSettings Settings { get; set; }

        #region Public Methods and Operators

        public void Configuration(IAppBuilder app)
        {
            SignatureConversions.AddConversions(app);
            Settings = CreateSettings();
            var config = CreateHttpConfiguration();
            Start(app, CreateContainer(app, config), config);
        }

        #endregion

        protected virtual INuGetWebApiSettings CreateSettings()
        {
            return new NuGetWebApiSettings();
        }


        protected virtual void Start(IAppBuilder app, IContainer container, HttpConfiguration config)
        {
            ConfigureWebApi(config, container);

            if (Settings.ShowExceptionDetails)
                app.UseErrorPage(new ErrorPageOptions
                {
                    ShowExceptionDetails = true,
                    ShowSourceCode = true
                });

            app.UseAutofacMiddleware(container);
            app.UseStageMarker(PipelineStage.Authenticate);

            //app.UseAutofacWebApi(config);
            app.UseWebApi(config);

            app.UseStageMarker(PipelineStage.MapHandler);

            RegisterServices(container, app, config);

            RegisterShutdown(app, container);
        }

        protected virtual HttpConfiguration CreateHttpConfiguration()
        {
            return new HttpConfiguration();
        }

        protected virtual void RegisterShutdown(IAppBuilder app, IContainer container)
        {
            var token = app.GetHostAppDisposing();

            if (token.CanBeCanceled)
                token.Register(() => OnShutdown(container));
            else
                Log.Warn(m => m("OWIN property host.OnAppDisposing not available."));
        }

        private async void OnShutdown(IContainer container)
        {
            try
            {
                await ShutdownServices(container);
            }
            finally
            {
                shutdownSignal.Set();
            }
        }

        protected virtual async Task ShutdownServices(IContainer container)
        {
            Log.Info(m => m("Received OnAppDisposing event from OWIN container."));

            var taskRunner = container.Resolve<ITaskRunner>();
            var pendingTasks = taskRunner.PendingTasks;
            if (pendingTasks.Length > 0)
            {
                Log.Info(m => m("Waiting for {0} background tasks.", pendingTasks.Length));
                await Task.WhenAll(pendingTasks);
            }

            Log.Info(m => m("Disposing Autofac application container."));
            container.Dispose();
        }

        protected virtual IContainer CreateContainer(IAppBuilder app, HttpConfiguration config)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new OwinAppLifecycleModule(app));
            builder.RegisterModule(new NuGetMultiRepositoryWebApiModule(Settings));
            builder.RegisterModule(new NuservModule(config));


            return builder.Build();
        }

        protected virtual void RegisterServices(IContainer container, IAppBuilder app, HttpConfiguration config)
        {
            var routeMapper = container.Resolve<NuGetMultiRepositoryWebApiRouteMapper>();

            routeMapper.MapApiRoutes(config);
            routeMapper.MapSymbolSourceRoutes(config);
            routeMapper.MapDataServiceRoutes(config);
        }

        protected virtual void ConfigureWebApi(HttpConfiguration config, IContainer container)
        {
            config.IncludeErrorDetailPolicy = Settings.ShowExceptionDetails
                ? IncludeErrorDetailPolicy.Always
                : IncludeErrorDetailPolicy.Default;

            config.MessageHandlers.Add(new CrossOriginMessageHandler(Settings.EnableCrossDomainRequests));
            config.Filters.Add(new CacheControlFilter());
            config.Filters.Add(new ExceptionLoggingFilter());

            var documentation = new HtmlDocumentation();
            documentation.Load();
            config.Services.Replace(typeof(IDocumentationProvider), new WebApiHtmlDocumentationProvider(documentation));
            config.Services.Replace(typeof(IExceptionHandler), new LoggingExceptionHandler());

            var formatter = CreateMicrodataFormatter();

            config.Formatters.Add(formatter);
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            var multiRepositoryPackageFormDataMediaFormatter = container.Resolve<MultiRepositoryPackageFormDataMediaFormatter>();
            config.Formatters.Add(multiRepositoryPackageFormDataMediaFormatter);

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
            config.DependencyResolver = container.Resolve<NuserveDependencyResolver>();
            GlobalConfiguration.Configuration.DependencyResolver = config.DependencyResolver;
        }

        protected virtual NuGetHtmlMicrodataFormatter CreateMicrodataFormatter()
        {
            var formatter = new NuGetHtmlMicrodataFormatter();
            formatter.SupportedMediaTypes.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            formatter.SupportedMediaTypes.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
            return formatter;
        }

        private class LoggingExceptionHandler : IExceptionHandler
        {
            private static readonly Task CompletedTask;

            static LoggingExceptionHandler()
            {
                var tcs = new TaskCompletionSource<bool>();
                tcs.SetResult(true);
                CompletedTask = tcs.Task;
            }

            public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
            {
                UnhandledExceptionLogger.LogException(context.Exception);
                return CompletedTask;
            }
        }
    }
}