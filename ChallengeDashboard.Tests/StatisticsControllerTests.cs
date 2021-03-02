using System.Threading.Tasks;
using FLRC.ChallengeDashboard.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class StatisticsControllerTests
    {
        [Fact]
        public async Task CanGetStatisticsForCourse()
        {
            //arrange
            var dataService = Substitute.For<IDataService>();
            var course = new Course {Results = CourseData.Results, Meters = 10000 };
            dataService.GetAllResults().Returns(new[] {course});
            var controller = new StatisticsController(dataService);
            
            //act
            var response = await controller.Index();
            
            //assert
            var vm = (StatisticsViewModel) response.Model;
            var stats = vm.Courses[course];
            Assert.Equal(4, stats.Participants[string.Empty]);
            Assert.Equal(8, stats.Runs[string.Empty]);
            Assert.Equal(8 * 10000 / Course.MetersPerMile, stats.Miles[string.Empty]);
            Assert.Equal(2, stats.Average[string.Empty]);

        }
    }
}