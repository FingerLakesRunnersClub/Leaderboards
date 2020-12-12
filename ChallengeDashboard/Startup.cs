using System.Collections.Generic;
using System.Threading.Tasks;
using FLRC.ChallengeDashboard.AgeGrade;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FLRC.ChallengeDashboard
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddHttpClient();
            services.AddSingleton<IDataAPI, DataAPI>();
            services.AddSingleton<IDataService, DataService>();
            services.AddSingleton(Loader.Load().GetAwaiter().GetResult());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints => endpoints.MapControllerRoute("default", "{controller=Dashboard}/{action=Index}/{id?}"));
        }
    }
}