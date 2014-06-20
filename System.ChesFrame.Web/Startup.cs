using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(System.ChesFrame.Web.Startup))]
namespace System.ChesFrame.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
