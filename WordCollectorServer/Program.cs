﻿using System;
using Microsoft.Owin.Hosting;
using Microsoft.AspNet.SignalR;

namespace WordCollectorServer
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            // This will *ONLY* bind to localhost, if you want to bind to all addresses
            // use http://*:8080 to bind to all addresses. 
            // See http://msdn.microsoft.com/en-us/library/system.net.httplistener.aspx 
            // for more information.
            string url = "http://localhost:8080";
            using (WebApp.Start(url))
            {
                //Game g = new Game(null, null);
                Console.WriteLine("Server running on {0}", url);
                Console.ReadLine();
            }
        }
    }
}
