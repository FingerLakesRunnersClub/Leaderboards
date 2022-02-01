using FLRC.Leaderboards.Core;

namespace FLRC.Leaderboards.Challenge;

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

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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