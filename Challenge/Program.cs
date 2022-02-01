namespace FLRC.Leaderboards.Challenge;

public static class Program
{
	public static void Main(string[] args)
		=> Host.CreateDefaultBuilder(args)
			.ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
			.Build().Run();
}