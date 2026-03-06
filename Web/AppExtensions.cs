using FLRC.Leaderboards.Core.Auth;
using FLRC.Leaderboards.Data;
using FLRC.Leaderboards.Web.Areas.Admin.Policies;
using FLRC.Leaderboards.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace FLRC.Leaderboards.Web;

public static class AppExtensions
{
	extension(IServiceCollection services)
	{
		public void AddAuthentication()
		{
			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
			services.AddSingleton<IDiscourseAuthenticator>(s =>
			{
				var authSecret = s.GetService<IConfiguration>().GetValue<string>("DiscourseAuthSecret");
				return new DiscourseAuthenticator("https://forum.fingerlakesrunners.org", authSecret);
			});

			services.AddSingleton<IAuthService, AuthService>();
			services.AddSingleton<IWebScorerAuthenticator, WebScorerAuthenticator>();
			services.AddAuthorizationBuilder()
				.AddPolicy(nameof(Admin), b => b.Requirements.Add(new Admin()));
		}

		public void AddDatabase()
		{
			services.AddScoped(s =>
			{
				var connectionString = s.GetService<IConfiguration>().GetValue<string>("Database");
				return new NpgsqlConnection(connectionString);
			});
			services.AddDbContext<DB>((s, o) => o.UseNpgsql(s.GetService<NpgsqlConnection>()).UseLowerCaseNamingConvention());
		}
	}
}