using FLRC.Leaderboards.Web;

namespace FLRC.Leaderboards.Challenge;

public static class Program
{
	public static void Main(string[] args)
		=> Host.CreateDefaultBuilder(args)
			.ConfigureWebHostDefaults(app => app.UseStartup<Startup>().UseWebRoot(Path.Combine(AppContext.BaseDirectory, "wwwroot")))
			.Build()
			.Run();
}