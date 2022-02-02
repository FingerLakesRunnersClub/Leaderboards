using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Groups;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FLRC.Leaderboards.Web;

public class Startup
{
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddControllersWithViews();
		services.AddHttpClient();

		services.AddSingleton<IDataService, DataService>();
		services.AddSingleton<IGroupAPI, GroupAPI>();

		services.AddSingleton<UltraSignup>();
		services.AddSingleton<WebScorer>();

		services.AddSingleton<DataAPI<UltraSignup>>();
		services.AddSingleton<DataAPI<WebScorer>>();

		services.AddSingleton<IDictionary<string, IDataAPI>>(s => new Dictionary<string, IDataAPI>
		{
			{ nameof(UltraSignup), s.GetService<DataAPI<UltraSignup>>() },
			{ nameof(WebScorer), s.GetService<DataAPI<WebScorer>>() }
		});
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