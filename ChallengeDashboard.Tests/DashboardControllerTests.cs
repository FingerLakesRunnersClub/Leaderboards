using System.Threading.Tasks;
using FLRC.ChallengeDashboard.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class DashboardControllerTests
    {
        [Fact]
        public async Task CanDashboardGetsAllCourses()
        {
            //arrange
            var dataService = Substitute.For<IDataService>();
            var controller = new DashboardController(dataService);

            //act
            await controller.Index();

            //assert
            await dataService.Received().GetAllCourses();
        }
    }
}
