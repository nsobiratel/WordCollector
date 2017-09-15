using System;
using Microsoft.Owin;
using WordCollectorServer;
using Owin;

[assembly:OwinStartupAttribute(typeof(Startup))]
namespace WordCollectorServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}

