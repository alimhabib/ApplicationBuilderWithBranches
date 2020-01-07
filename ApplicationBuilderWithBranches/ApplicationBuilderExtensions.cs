using System; 

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationBuilderWithBranches {

    /// <summary>
    /// An extenstion class for <inheritdoc cref="IApplicationBuilder"/>
    /// </summary>
    public static class ApplicationBuilderExtensions {
        /// <summary>
        /// UseBranchWithServices 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="path"></param> 
        /// <param name="appBuilderConfiguration"></param>
        /// <param name="requiredStartup"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseBranchWithServices(
            this IApplicationBuilder app,
            PathString path,
            Action<IApplicationBuilder> appBuilderConfiguration,
            Type requiredStartup) {

            IWebHost webHost = new WebHostBuilder().UseKestrel()
                .ConfigureServices(s => {
                    //Include the Sub project as part of the pipeline and scan for any controllers 
                    s.AddMvc().AddApplicationPart(requiredStartup.Assembly)
                        .ConfigureApplicationPartManager(manager => {
                            manager.FeatureProviders.Clear();
                            manager.FeatureProviders.Add(
                                new TypedControllerFeatureProvider<ControllerBase>());
                        });
                })
                .UseStartup(requiredStartup)
                .Build();


            var serviceProvider = webHost.Services;
            var serverFeatures = webHost.ServerFeatures;


            var appBuilderFactory =
                serviceProvider.GetRequiredService<IApplicationBuilderFactory>();
            var branchBuilder = appBuilderFactory.CreateBuilder(serverFeatures);
            var factory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            branchBuilder.Use(async (context, next) => {
                using (var scope = factory.CreateScope()) {

                    //Adding the specific Dependencies 
                    context.RequestServices = scope.ServiceProvider;
                    await next().ConfigureAwait(false);
                }
            });

            appBuilderConfiguration(branchBuilder);
            var branchDelegate = branchBuilder.Build();

            return app.Map(path, builder => {
                builder.Use(async (context, next) => { 
                    //Call The Branch and execute the route 
                    await branchDelegate(context).ConfigureAwait(false);
                });
            });

        }

    }
}
