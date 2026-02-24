using FLRC.Leaderboards.Importer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices((_, services) =>
	{
		FLRC.Leaderboards.Web.App.ConfigureServices(services);
		services.AddSingleton<Action<string>>(Console.WriteLine);
		services.AddSingleton<Importer>();
		services.AddHostedService<App>();
	}
);

var app = builder.Build();
FLRC.Leaderboards.Web.App.Initialize(app.Services);

await app.StartAsync();
await app.StopAsync();
await app.WaitForShutdownAsync();