using System.IO.Abstractions;
using System.Text.Json;
using Microsoft.Extensions.Hosting;

namespace FLRC.Leaderboards.Importer;

public sealed class App(IFileSystem fs, Importer importer, Action<string> log) : IHostedService
{
	public async Task StartAsync(CancellationToken cancellationToken)
	{
		//await Run("Challenge", cancellationToken);
		await Run("TrailCircuit", cancellationToken);
	}

	private async Task Run(string series, CancellationToken cancellationToken)
	{
		log($"Starting {series}...");
		var data = await fs.File.ReadAllTextAsync($"{series}.json", cancellationToken);
		var courses = JsonSerializer.Deserialize<CourseImportConfig[]>(data)!;
		await importer.Run(courses);
		log($"Done {series}!");
	}

	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}