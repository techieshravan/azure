using Owin;
using Microsoft.Owin;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(SignalRChat.Startup))]
namespace SignalRChat
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            string connectionString = "Endpoint=sb://signalrscaleout1.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=jqZb9xKYuymL7wOlNV6wDIu/qlttPk3ENL4OAxHdzGA=";
            GlobalHost.DependencyResolver.UseServiceBus(connectionString, "Chat");
            app.MapSignalR();
        }
    }
}