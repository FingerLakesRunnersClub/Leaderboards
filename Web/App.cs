using FLRC.Leaderboards.Core;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Groups;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FLRC.Leaderboards.Web;

public static class App
{
	public static async Task Run(string[] args)
	{
		var options = new WebApplicationOptions
		{
			Args = args,
			WebRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot")
		};
		var builder = WebApplication.CreateBuilder(options);
		ConfigureServices(builder.Services);
		var app = builder.Build();
		Configure(app, app.Environment);
		await app.RunAsync();
	}

	public static void ConfigureServices(IServiceCollection services)
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
			{ nameof(UltraSignup), s.GetRequiredService<DataAPI<UltraSignup>>() },
			{ nameof(WebScorer), s.GetRequiredService<DataAPI<WebScorer>>() }
		});

		services.AddSingleton(s => new Config(s.GetRequiredService<IConfiguration>()));
	}

	public static void Configure(IApplicationBuilder app, IHostEnvironment env)
	{
		if (env.IsDevelopment())
			app.UseDeveloperExceptionPage();

		app.UseStaticFiles();
		app.UseRouting();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllerRoute("Leaderboard", "{controller=Leaderboard}/{action=Index}/{id?}");
			endpoints.MapControllerRoute("MultiCourseRaces", "{controller}/{id}/{distance}/{action}/{category?}");
			endpoints.MapControllerRoute("SingleCourseRaces", "{controller}/{id}/{action}/{category?}");
			endpoints.MapControllerRoute("Athlete", "{controller}/{id}/{action}/{courseID}/{distance?}");
			endpoints.MapControllerRoute("Default", "{controller}/{id}/{action}");
		});
	}
}