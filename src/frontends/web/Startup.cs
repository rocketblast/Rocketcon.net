using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using interfaces.Example;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Orleans;
using Orleans.Configuration;

namespace frontends
{
    public class Startup
    {
        private static readonly string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        private static ILoggerFactory loggerFactory;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            // Configure logging based on configuration
            loggerFactory = new LoggerFactory().AddConsole(Configuration.GetSection("Logging")).AddDebug();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IClusterClient>(CreateClusterClient);
            // Add caching here later on

            services.AddMvc().AddJsonOptions(op =>
            {
                op.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                op.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                op.SerializerSettings.Formatting = Formatting.Indented;

                JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    HotModuleReplacementServerPort = 5001,
                    ReactHotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseDefaultFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }

        #region Private methods
        private IClusterClient CreateClusterClient(IServiceProvider arg)
        {
            var client = new ClientBuilder()
                //.ConfigureApplicationParts(p => p.AddApplicationPart(typeof(IValueGrain).Assembly))
                .ConfigureApplicationParts(p => p.AddFromApplicationBaseDirectory())
                .Configure<ClusterOptions>(op =>
                {
                    op.ClusterId = "rocketcon-backend";
                    op.ServiceId = "rocketcon";
                })
                //.UseLocalhostClustering()
                .UseMongoDBClustering(op =>
                {
                    op.DatabaseName = "rocketcon";
                    op.ConnectionString = "mongodb://mongodb";
                }).Build();

            // Tries to startup a connection towards orleans backend
            StartClientWithRetries(client).Wait();

            return client;
        }

        /// <summary>
        /// Logic for reconnecting to orleans cluster, will
        /// default to 5 tries with 5seconds delay between each try
        /// </summary>
        /// <param name="client">Cluster client to use</param>
        /// <param name="tries">Number of retries</param>
        /// <param name="wait">How long to wait between each try</param>
        /// <returns></returns>
        private static async Task StartClientWithRetries(IClusterClient client, int tries = 5, int wait = 5)
        {
            for (var i = 0; i < tries; i++)
            {
                try
                {
                    await client.Connect();
                    return;
                }
                catch (Exception ex)
                {
                    // ??
                    // throw/log something here later on
                    throw ex;
                }

                await Task.Delay(TimeSpan.FromSeconds(wait));
            }
        }
        #endregion
    }
}
