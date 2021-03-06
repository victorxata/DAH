﻿using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using TechTracker.Api;

[assembly: OwinStartup(typeof(Startup))]

namespace TechTracker.Api
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            ConfigureAuth(app);
        }
    }
}
