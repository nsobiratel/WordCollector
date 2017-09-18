using System;
using Microsoft.Owin;
using WordCollectorServer;
using Owin;
using Microsoft.Owin.Cors;

[assembly:OwinStartupAttribute(typeof(Startup))]
namespace WordCollectorServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
}

