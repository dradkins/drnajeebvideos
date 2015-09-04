using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DrNajeeb.Web.Startup))]
namespace DrNajeeb.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
