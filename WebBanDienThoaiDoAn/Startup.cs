using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebBanDienThoaiDoAn.Startup))]
namespace WebBanDienThoaiDoAn
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
