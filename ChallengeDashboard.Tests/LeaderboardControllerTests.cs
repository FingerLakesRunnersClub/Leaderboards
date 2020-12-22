using System.Threading.Tasks;
using FLRC.ChallengeDashboard.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class LeaderboardControllerTests
    {
        [Fact]
        public async Task CanGetAllCourses()
        {
            //arrange
            var dataService = Substitute.For<IDataService>();
            var controller = new LeaderboardController(dataService);

            //act
            await controller.Index();

            //assert
            await dataService.Received().GetAllResults();
        }
    }
}
