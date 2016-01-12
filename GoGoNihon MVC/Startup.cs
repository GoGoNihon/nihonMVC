using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GoGoNihon_MVC.Startup))]
namespace GoGoNihon_MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
