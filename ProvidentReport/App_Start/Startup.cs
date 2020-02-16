﻿using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using ProvidentReport;

[assembly: OwinStartup(typeof(Startup))]

namespace ProvidentReport
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new PathString("/Auth/Login")
            });
            
        }
    }
}
