using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Poloneix.Web.Startup))]
namespace Poloneix.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
