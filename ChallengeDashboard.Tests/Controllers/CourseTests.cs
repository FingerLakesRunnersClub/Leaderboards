using System.Threading.Tasks;
using FLRC.ChallengeDashboard.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests.Controllers
{
    public class CourseTests
    {
        [Fact]
        public async Task ResultsGetCourseInfo()
        {
            //arrange
            var dataService = Substitute.For<IDataService>();
            var controller = new CourseController(dataService);

            //act
            var response = await controller.Index(123);

            //assert
            await dataService.Received().GetCourse(123);
        }
    }
}
