using System.IO.Abstractions;
using System.Text.Json;
using Microsoft.Extensions.Hosting;

namespace FLRC.Leaderboards.Importer;

public sealed class App(IFileSystem fs, Importer importer, Action<string> log) : IHostedService
{
	public async Task StartAsync(CancellationToken cancellationToken)
	{
		log("Starting...");
		var data = await fs.File.ReadAllTextAsync("TrailCircuit.json", cancellationToken);
		var courses = JsonSerializer.Deserialize<CourseImportConfig[]>(data)!;
		await importer.Run(courses);
		log("Done!");
	}

	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}