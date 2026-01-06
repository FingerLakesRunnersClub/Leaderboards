using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests;

public sealed class AppTests
{
	[Fact]
	public void CanConfigureServices()
	{
		//arrange
		var services = new ServiceCollection();

		//act
		App.ConfigureServices(services);

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

		App.ConfigureServices(services);

		var serviceProvider = services.BuildServiceProvider();
		var app = new ApplicationBuilder(serviceProvider);

		//act
		App.Configure(app);

		//assert
		Assert.NotEmpty(app.Properties);
	}
}