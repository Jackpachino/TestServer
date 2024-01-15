using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Forex_Update.Startup))]
namespace Forex_Update
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
