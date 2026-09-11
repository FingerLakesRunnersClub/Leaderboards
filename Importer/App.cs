using System.IO.Abstractions;
using System.Text.Json;
using FLRC.Leaderboards.Core.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FLRC.Leaderboards.Importer;

public sealed class App(IFileSystem fs, Importer importer, Action<string> log) : IHostedService
{
	public static void Services(HostBuilderContext context, IServiceCollection services)
	{
		Web.App.ConfigureServices(services);
		services.AddSingleton<IContextProvider>(_ => new AppContextProvider("Challenge"));
		services.AddSingleton<Action<string>>(Console.WriteLine);
		services.AddSingleton<Importer>();
		services.AddHostedService<App>();
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		await Run("Challenge");
		// await Run("TrailCircuit");
	}

	private async Task Run(string series)
	{
		log($"Starting {series}...");
		var data = await fs.File.ReadAllTextAsync($"{series}.json");
		var courses = JsonSerializer.Deserialize<CourseImportConfig[]>(data)!;
		await importer.Run(courses);
		log($"Done {series}!");
	}

	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}