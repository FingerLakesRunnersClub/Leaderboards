using System.IO.Abstractions;
using FLRC.Leaderboards.Core.Auth;
using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Data.Migrations;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Areas.Admin.Policies;
using FLRC.Leaderboards.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace FLRC.Leaderboards.Web;

public sealed class App(string context)
{
	public async Task Run(string[] args)
	{
		var options = new WebApplicationOptions
		{
			Args = args,
			WebRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot")
		};
		var builder = WebApplication.CreateBuilder(options);
		builder.Configuration.AddJsonFile($"{context}.json");

		ConfigureServices(builder.Services);
		builder.Services.AddSingleton<IContextProvider>(_ => new AppContextProvider(context));
		var app = builder.Build();

		Configure(app);
		Initialize(app.Services);

		await app.RunAsync();
	}

	public static void ConfigureServices(IServiceCollection services)
	{
		services.AddDatabase();

		services.AddControllersWithViews();
		services.AddHttpClient();
		services.AddHttpContextAccessor();

		services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(AddCookieOptions);
		services.AddAuthorizationBuilder().AddPolicy(nameof(Admin), AddAdminRequirement);

		services.AddSingleton<IDiscourseFactory, DiscourseFactory>();
		services.AddSingleton<IDiscourseAuthenticator, DiscourseAuthenticator>();
		services.AddSingleton<IAuthService, AuthService>();
		services.AddSingleton<IWebScorerAuthenticator, WebScorerAuthenticator>();

		services.AddSingleton<IConfig, AppConfig>();

		services.AddSingleton<IDataService, DataService>();
		services.AddSingleton<ICommunityAPI, DiscourseAPI>();
		services.AddSingleton<ICustomInfoAPI, CustomInfoAPI>();

		services.AddSingleton<UltraSignup>();
		services.AddSingleton<WebScorer>();
		services.AddSingleton<WebScorerStartList>();

		services.AddSingleton<IFileSystem, FileSystem>();
		services.AddSingleton<IFileSystemResultsLoader, FileSystemResultsLoader>();

		services.AddSingleton<ResultsAPI<UltraSignup>>();
		services.AddSingleton<ResultsAPI<WebScorer>>();
		services.AddSingleton<ResultsAPI<WebScorerStartList>>();
		services.AddSingleton(ResultsAPI);

		services.AddScoped<IAdminService, AdminService>();
		services.AddScoped<IAthleteService, AthleteService>();
		services.AddScoped<IChallengeService, ChallengeService>();
		services.AddScoped<ICourseService, CourseService>();
		services.AddScoped<IIterationService, IterationService>();
		services.AddScoped<IRaceService, RaceService>();
		services.AddScoped<IResultService, ResultService>();
		services.AddScoped<ISeriesService, SeriesService>();

		services.AddScoped<IContextManager, ContextManager>();
		services.AddScoped<IImportManager, ImportManager>();
		services.AddScoped<ILegacyDataConverter, LegacyDataConverter>();
		services.AddScoped<IIterationManager, IterationManager>();
		services.AddScoped<IRegistrationManager, RegistrationManager>();

		services.AddScoped<IAthleteSummaryCalculator, AthleteSummaryCalculator>();
	}

	private static IDictionary<string, IResultsAPI> ResultsAPI(IServiceProvider s) =>
		new Dictionary<string, IResultsAPI>
		{
			{ nameof(UltraSignup), s.GetService<ResultsAPI<UltraSignup>>() },
			{ nameof(WebScorer), s.GetService<ResultsAPI<WebScorer>>() },
			{ nameof(WebScorerStartList), s.GetService<ResultsAPI<WebScorerStartList>>() }
		};

	private static void AddAdminRequirement(AuthorizationPolicyBuilder policyBuilder)
		=> policyBuilder.Requirements.Add(new Admin());

	private static void AddCookieOptions(CookieAuthenticationOptions options)
	{
		options.ExpireTimeSpan = TimeSpan.FromDays(30);
		options.SlidingExpiration = true;
	}

	public static void Configure(IApplicationBuilder app)
	{
		app.UseExceptionHandler("/error");
		app.UseStatusCodePagesWithReExecute("/error");

		app.UseStaticFiles();
		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapAreaControllerRoute("Admin", "Admin", "/Admin/{controller=Home}/{action=Index}/{id?}");
			endpoints.MapControllerRoute("Leaderboard", "{controller=Leaderboard}/{action=Index}/{id?}");
			endpoints.MapControllerRoute("Athlete", "{controller}/{id}/{action}/{courseID}/{name?}");
			endpoints.MapControllerRoute("Course", "{controller}/{id}/{name}/{action}/{category?}");
			endpoints.MapControllerRoute("Default", "{controller}/{id}/{action}");
			endpoints.MapDefaultControllerRoute();
		});
	}

	public static void Initialize(IServiceProvider serviceProvider)
	{
		var connection = serviceProvider.GetService<NpgsqlConnection>();
		var logFactory = serviceProvider.GetService<ILoggerFactory>();
		var logger = logFactory.CreateLogger("Initializer");
		var upgrader = new DBUpgrader(connection, logger);
		upgrader.MigrateDatabase();
	}
}