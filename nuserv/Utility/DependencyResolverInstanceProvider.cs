namespace nuserv.Utility
{
    #region Usings

    using System;
    using System.ServiceModel;
    using System.ServiceModel.Dispatcher;
    using System.Web.Http;

    #endregion

    public class DependencyResolverInstanceProvider : IInstanceProvider
    {
        #region Fields

        private readonly Type serviceType;

        #endregion

        #region Constructors and Destructors

        public DependencyResolverInstanceProvider(Type serviceType)
        {
            this.serviceType = serviceType;
        }

        #endregion

        #region Public Methods and Operators

        public object GetInstance(InstanceContext instanceContext, System.ServiceModel.Channels.Message message)
        {
            return this.GetInstance(instanceContext);
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            var dependencyScope = GlobalConfiguration.Configuration.DependencyResolver.BeginScope();

            instanceContext.Extensions.Add(new DependencyResolverInstanceProviderContextScope(dependencyScope));

            return dependencyScope.GetService(this.serviceType);
        }

        public void ReleaseInstance(System.ServiceModel.InstanceContext instanceContext, object instance)
        {
            instanceContext.Extensions.Find<DependencyResolverInstanceProviderContextScope>().Dispose();
        }

        #endregion

        private class DependencyResolverInstanceProviderContextScope : IExtension<InstanceContext>, IDisposable
        {
            #region Fields

            private readonly IDisposable disposable;

            #endregion

            #region Constructors and Destructors

            public DependencyResolverInstanceProviderContextScope(IDisposable disposable)
            {
                this.disposable = disposable;
            }

            #endregion

            #region Public Methods and Operators

            public void Attach(InstanceContext owner)
            {
            }

            public void Detach(InstanceContext owner)
            {
            }

            public void Dispose()
            {
                this.disposable.Dispose();
            }

            #endregion
        }
    }
}