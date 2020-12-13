using System.Collections.Generic;
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
            dataService.GetCourse(Arg.Any<uint>()).Returns(new Course { Results = new List<Result>() });
            var controller = new CourseController(dataService);

            //act
            var response = await controller.Fastest(123);

            //assert
            await dataService.Received().GetCourse(123);
        }
    }
}
