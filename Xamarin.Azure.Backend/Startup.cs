using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Xamarin.Azure.Backend.Startup))]

namespace Xamarin.Azure.Backend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}