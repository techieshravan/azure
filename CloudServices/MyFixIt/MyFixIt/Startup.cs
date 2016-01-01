using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyFixIt.Startup))]
namespace MyFixIt
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
