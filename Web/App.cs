using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace FLRC.Leaderboards.Web;

public static class App
{
	public static void Run(string[] args)
		=> Host.CreateDefaultBuilder(args)
			.ConfigureWebHostDefaults(app => app.UseStartup<Startup>().UseWebRoot(Path.Combine(AppContext.BaseDirectory, "wwwroot")))
			.Build()
			.Run();
}