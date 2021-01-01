using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class TeamControllerTests
    {
        [Fact]
        public async Task CanGetTeamNameFromViewModel()
        {
            //arrange
            var dataService = Substitute.For<IDataService>();
            var controller = new TeamController(dataService);

            //act
            var response = await controller.Index(3);

            //assert
            var vm = (TeamSummaryViewModel)response.Model;
            Assert.Equal("30–39", vm.Team.Display);
        }

        [Fact]
        public async Task CanGetOverallResultsForTeam()
        {
            //arrange
            var dataService = Substitute.For<IDataService>();
            dataService.GetAllResults().Returns(LeaderboardData.Courses);
            var controller = new TeamController(dataService);

            //act
            var response = await controller.Index(3);

            //assert
            var vm = (TeamSummaryViewModel)response.Model;
            Assert.Equal(1, vm.Overall.Rank.Value);
        }

        [Fact]
        public async Task CanGetCourseResultsForTeam()
        {
            //arrange
            var dataService = Substitute.For<IDataService>();
            dataService.GetAllResults().Returns(LeaderboardData.Courses);
            var controller = new TeamController(dataService);

            //act
            var response = await controller.Index(3);

            //assert
            var vm = (TeamSummaryViewModel)response.Model;
            Assert.Equal(1, vm.Courses.First().Value.Rank.Value);
        }
    }
}