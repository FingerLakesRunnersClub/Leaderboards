using System.Threading.Tasks;
using FLRC.ChallengeDashboard.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class CompletedControllerTests
    {
        [Fact]
        public async Task CanGetListOfAthletes()
        {
            //arrange
            var course = new Course
            {
                Results = CourseData.Results
            };
            var dataService = Substitute.For<IDataService>();
            dataService.GetAllResults().Returns(new[] { course });

            var controller = new CompletedController(dataService);

            //act
            var response = await controller.Index();

            //assert
            var vm = (CompletedViewModel)response.Model;
            Assert.Equal(4, vm.RankedResults.Count);
        }
    }
}