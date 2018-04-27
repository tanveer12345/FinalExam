using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(comp2007amMusicStore.Startup))]
namespace comp2007amMusicStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
