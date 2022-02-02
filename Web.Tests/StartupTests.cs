using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web;

public class StartupTests
{
	[Fact]
	public void CanConfigureServices()
	{
		//arrange
		var services = new ServiceCollection();
		var startup = new Startup();

		//act
		startup.ConfigureServices(services);

		//assert
		Assert.NotEmpty(services);
	}

	[Fact]
	public void CanConfigureApp()
	{
		//arrange
		var services = new ServiceCollection();
		var diag = new DiagnosticListener("test");
		services.AddSingleton<DiagnosticSource>(_ => diag);
		services.AddSingleton(_ => diag);

		var startup = new Startup();
		startup.ConfigureServices(services);

		var serviceProvider = services.BuildServiceProvider();
		var app = new ApplicationBuilder(serviceProvider);

		var env = Substitute.For<IHostEnvironment>();

		//act
		startup.Configure(app, env);

		//assert
		Assert.NotEmpty(app.Properties);
	}
}