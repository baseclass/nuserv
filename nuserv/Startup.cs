using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(nuserv.Startup))]
namespace nuserv
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
