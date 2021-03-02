using FLRC.ChallengeDashboard.Controllers;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class AboutControllerTests
    {
        [Fact]
        public void IndexRedirectsToSetURL()
        {
            //arrange
            var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
            var controller = new AboutController(config);
            
            //act
            var result = controller.Index();
            
            //assert
            Assert.Equal("https://example.com", result.Url);
        }
        
        [Fact]
        public void RulesRedirectsToSetURL()
        {
            //arrange
            var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
            var controller = new AboutController(config);
            
            //act
            var result = controller.Rules();
            
            //assert
            Assert.Equal("https://example.com#rules", result.Url);
        }
        
        [Fact]
        public void TimingRedirectsToSetURL()
        {
            //arrange
            var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
            var controller = new AboutController(config);
            
            //act
            var result = controller.Timing();
            
            //assert
            Assert.Equal("https://example.com#timing", result.Url);
        }
    }
}