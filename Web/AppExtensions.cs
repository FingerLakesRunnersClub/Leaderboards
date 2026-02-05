using System.Data;
using FLRC.Leaderboards.Core.Auth;
using FLRC.Leaderboards.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace FLRC.Leaderboards.Web;

public static class AppExtensions
{
	extension(IServiceCollection services)
	{
		public void AddDiscourseAuthentication()
		{
			var authSecret = Environment.GetEnvironmentVariable("DiscourseAuthSecret");
			if (string.IsNullOrWhiteSpace(authSecret))
				return;

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
			var authenticator = new DiscourseAuthenticator("https://forum.fingerlakesrunners.org", authSecret);
			services.AddSingleton<IDiscourseAuthenticator>(authenticator);

			services.AddAuthorization();
		}

		public void AddDatabase()
		{
			var connectionString = Environment.GetEnvironmentVariable("Database");
			if (string.IsNullOrWhiteSpace(connectionString))
				return;

			var connection = new NpgsqlConnection(connectionString);
			services.AddDbContext<DB>(o => o.UseNpgsql(connection).UseLowerCaseNamingConvention());
			services.AddScoped<IDbConnection>(_ => connection);
		}
	}
}