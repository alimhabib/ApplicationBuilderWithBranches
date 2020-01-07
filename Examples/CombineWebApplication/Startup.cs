using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationBuilderWithBranches;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CombineWebApplication {
    public class Startup {
        public Startup(IHostingEnvironment env) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseBranchWithServices("/admin" ,
                a => {
                    a.Use(async (c, next) => {
                        if (c.Request.Path.ToString().Contains("foo")) {
                            await c.Response.WriteAsync("bar!");
                        } else {
                            await next();
                        }
                    });

                    a.UseMvc();
                } , typeof( AdminWebApplication.Startup));

            app.UseBranchWithServices("/api", 
                a => {
                    a.UseMvc();
                } , typeof(ResourcesWebApplication.Startup));

            app.Run(async c => {
                await c.Response.WriteAsync("Nothing here!");
            });
        }
    }
}
