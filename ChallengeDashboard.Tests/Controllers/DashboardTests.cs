using System.Collections.Generic;
using System.Threading.Tasks;
using FLRC.ChallengeDashboard.Controllers;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests.Controllers
{
    public class DashboardTests
    {
        [Fact]
        public async Task CanDashboardGetsAllCourses()
        {
            //arrange
            var config = Substitute.For<IConfiguration>();
            var dataService = Substitute.For<IDataService>();
            var controller = new DashboardController(config, dataService);

            //act
            await controller.Index();

            //assert
            await dataService.Received().GetAllCourses(Arg.Any<IEnumerable<uint>>());
        }
    }
}
