using FLRC.Leaderboards.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FLRC.Leaderboards.Web;

public class Startup
{
	public IConfiguration Configuration { get; }

	public Startup(IConfiguration configuration) => Configuration = configuration;

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddControllersWithViews();
		services.AddHttpClient();
		services.AddSingleton<IDataAPI, DataAPI>();
		services.AddSingleton<IGroupAPI, GroupAPI>();
		services.AddSingleton<IDataService, DataService>();
	}

	public void Configure(IApplicationBuilder app, IHostEnvironment env)
	{
		if (env.IsDevelopment())
			app.UseDeveloperExceptionPage();

		app.UseStaticFiles();
		app.UseRouting();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllerRoute("Leaderboard", "{controller=Leaderboard}/{action=Index}/{id?}");
			endpoints.MapControllerRoute("Courses", "{controller}/{id}/{action}/{other?}");
		});
	}
}