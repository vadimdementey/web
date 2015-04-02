using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;
using System.Reflection;
using CoreLibrary;
using SqlLibrary;

namespace WebApp
{
    public class Startup
    {
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                    .AddMvc()
                    .AddConfig("config/plugins.json")
                    .AddCustomMvc()
                    .AddCustomController<HomeController>()
                    .AddCustomController<SqlController>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc(route =>
            {
               route.MapRoute(name: "default", template: "{controller}/{action}/{id?}", defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
