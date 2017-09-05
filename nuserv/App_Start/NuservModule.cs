using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using nuserv.Controllers;
using nuserv.Models;
using nuserv.Models.Contracts;
using nuserv.Service;
using nuserv.Service.Contracts;
using nuserv.Utility;
using NuGet;
using NuGet.Lucene;
using NuGet.Lucene.IO;

namespace nuserv
{
    public class NuservModule : Module
    {
        private readonly HttpConfiguration config;

        public NuservModule(HttpConfiguration config)
        {
            this.config = config;
        }

        #region Methods

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterInstance(config);

            builder.RegisterType<LifetimeScopeFactory>().As<ILifetimeScopeFactory>().SingleInstance();

            builder.RegisterType<LifetimeScopeResolver>().As<ILifetimeScopeResolver>().SingleInstance();
            builder.RegisterType<RepositoryLifetimeScopeService>().As<IRepositoryLifetimeScopeService>().SingleInstance();
            builder.RegisterType<HttpRouteDataResolver>().As<IHttpRouteDataResolver>().SingleInstance();
            builder.RegisterType<RepositoryManager>().As<IRepositoryManager>().SingleInstance();
            builder.RegisterType<RepositoryRepository>().As<IRepositoryRepository>();
            builder.RegisterType<NuserveDependencyResolver>();
            builder.RegisterType<MultiRepositoryPackageFormDataMediaFormatter>();
            builder.RegisterType<WebApi.Controllers.RepositoryController>();

            builder.RegisterType<Repository>().As<IRepository>();
            builder.RegisterType<RepositoryFactory>().As<IRepositoryFactory>().SingleInstance();
        }

        #endregion
    }
}