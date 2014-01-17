#region Usings

using Microsoft.Owin;

#endregion

[assembly: OwinStartup(typeof(nuserv.Startup))]

namespace nuserv
{
    #region Usings

    using Owin;

    #endregion

    public partial class Startup
    {
        #region Public Methods and Operators

        public void Configuration(IAppBuilder app)
        {
            this.ConfigureAuth(app);
        }

        #endregion
    }
}