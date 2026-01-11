using System.Data;
using System.IO.Abstractions;
using FLRC.Leaderboards.Core.Auth;
using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Series;
using FLRC.Leaderboards.Data;
using FLRC.Leaderboards.Data.Migrations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

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
		Configure(app);
		Initialize(app);
		await app.Services.GetRequiredService<IDataService>().GetAthletes();
		await app.RunAsync();
	}

	private static void Initialize(IApplicationBuilder app)
	{
		var connection = app.ApplicationServices.GetService<IDbConnection>();
		if (connection is null)
			return;

		var logFactory = app.ApplicationServices.GetService<ILoggerFactory>();
		var logger = logFactory.CreateLogger("Initializer");
		var upgrader = new DBUpgrader(connection, logger);
		upgrader.MigrateDatabase();
	}

	public static void ConfigureServices(IServiceCollection services)
	{
		services.AddControllersWithViews();
		services.AddHttpClient();
		services.AddHttpContextAccessor();

		services.AddDatabase();
		services.AddDiscourseAuthentication();

		services.AddSingleton<IConfig, AppConfig>();

		services.AddSingleton<IDataService, DataService>();
		services.AddSingleton<ICommunityAPI, DiscourseAPI>();
		services.AddSingleton<ICustomInfoAPI, CustomInfoAPI>();
		services.AddSingleton<ISeriesManager, SeriesManager>();

		services.AddSingleton<UltraSignup>();
		services.AddSingleton<WebScorer>();

		services.AddSingleton<IFileSystem, FileSystem>();
		services.AddSingleton<IFileSystemResultsLoader, FileSystemResultsLoader>();

		services.AddSingleton<ResultsAPI<UltraSignup>>();
		services.AddSingleton<ResultsAPI<WebScorer>>();

		services.AddSingleton<IDictionary<string, IResultsAPI>>(s => new Dictionary<string, IResultsAPI>
		{
			{ nameof(UltraSignup), s.GetRequiredService<ResultsAPI<UltraSignup>>() },
			{ nameof(WebScorer), s.GetRequiredService<ResultsAPI<WebScorer>>() }
		});
	}

	public static void Configure(IApplicationBuilder app)
	{
		app.UseExceptionHandler("/error");
		app.UseStatusCodePagesWithReExecute("/error");

		app.UseStaticFiles();
		app.UseRouting();
		app.UseAuthentication();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllerRoute("Leaderboard", "{controller=Leaderboard}/{action=Index}/{id?}");
			endpoints.MapControllerRoute("Athlete", "{controller}/{id}/{action}/{courseID}/{name?}");
			endpoints.MapControllerRoute("Course", "{controller}/{id}/{name}/{action}/{category?}");
			endpoints.MapControllerRoute("Default", "{controller}/{id}/{action}");
		});
	}

	extension(IServiceCollection services)
	{
		private void AddDiscourseAuthentication()
		{
			var authSecret = Environment.GetEnvironmentVariable("DiscourseAuthSecret");
			if (string.IsNullOrWhiteSpace(authSecret))
				return;

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
			var authenticator = new DiscourseAuthenticator("https://forum.fingerlakesrunners.org", authSecret);
			services.AddSingleton<IDiscourseAuthenticator>(authenticator);
		}

		private void AddDatabase()
		{
			var connectionString = Environment.GetEnvironmentVariable("Database");
			if (string.IsNullOrWhiteSpace(connectionString))
				return;

			var connection = new NpgsqlConnection(connectionString);
			services.AddDbContext<DB, DB>(o => o.UseNpgsql(connection).UseLowerCaseNamingConvention());
			services.AddScoped<IDbConnection>(_ => connection);
		}
	}
}