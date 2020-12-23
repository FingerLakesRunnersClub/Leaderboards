using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using NSubstitute;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class StartupTests
    {
        [Fact]
        public void CanConfigureServices()
        {
            //arrange
            var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
            var services = new ServiceCollection();
            var startup = new Startup(config);

            //act
            startup.ConfigureServices(services);

            //assert
            Assert.NotEmpty(services);
        }

        [Fact]
        public void CanConfigureApp()
        {
            //arrange
            var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
            var services = new ServiceCollection();
            var startup = new Startup(config);
            startup.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            var app = new ApplicationBuilder(serviceProvider);
            var env = Substitute.For<IWebHostEnvironment>(); 

            //act
            startup.Configure(app, env);
            
            //assert
            Assert.NotEmpty(app.Properties);
        }
    }
}