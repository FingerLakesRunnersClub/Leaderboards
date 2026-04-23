using FLRC.Leaderboards.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace FLRC.Leaderboards.Web;

public static class AppExtensions
{
	extension(IServiceCollection services)
	{
		public void AddDatabase()
		{
			services.AddScoped(s =>
			{
				var connectionString = s.GetRequiredService<IConfiguration>().GetValue<string>("Database");
				return new NpgsqlConnection(connectionString);
			});
			services.AddDbContext<DB>((s, o) => o.UseLazyLoadingProxies().UseNpgsql(s.GetService<NpgsqlConnection>()).UseLowerCaseNamingConvention());
		}
	}
}