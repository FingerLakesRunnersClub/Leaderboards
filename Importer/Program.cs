using FLRC.Leaderboards.Importer;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices(App.Services);

var app = builder.Build();
FLRC.Leaderboards.Web.App.Initialize(app.Services);

await app.StartAsync();
await app.StopAsync();
await app.WaitForShutdownAsync();