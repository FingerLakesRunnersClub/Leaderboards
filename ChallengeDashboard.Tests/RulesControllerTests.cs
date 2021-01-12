using FLRC.ChallengeDashboard.Controllers;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class RulesControllerTests
    {
        [Fact]
        public void RedirectsToSetURL()
        {
            //arrange
            var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
            var controller = new RulesController(config);
            
            //act
            var result = controller.Index();
            
            //assert
            Assert.Equal("https://example.com", result.Url);
        }
    }
}