using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FLRC.ChallengeDashboard.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class CourseControllerTests
    {
        [Fact]
        public async Task ResultsGetCourseInfo()
        {
            //arrange
            var dataService = Substitute.For<IDataService>();
            dataService.GetCourse(Arg.Any<uint>()).Returns(new Course { Results = new List<Result>() });
            var controller = new CourseController(dataService);

            //act
            await controller.Fastest(123);

            //assert
            await dataService.Received().GetCourse(123);
        }

        [Fact]
        public async Task CanGetFastestResults()
        {
            //arrange
            var course = Substitute.For<Course>();
            course.Results = new List<Result>
            {
                new Result { Athlete = new Athlete { ID = 123 }, Duration = TimeSpan.Parse("2:34") },
                new Result { Athlete = new Athlete { ID = 234 }, Duration = TimeSpan.Parse("1:23") },
            };

            var dataService = Substitute.For<IDataService>();
            dataService.GetCourse(Arg.Any<uint>()).Returns(course);

            var controller = new CourseController(dataService);

            //act
            var response = await controller.Fastest(123);

            //assert
            Assert.Equal((uint)234, ((ResultsViewModel<TimeSpan>)(response.Model)).RankedResults.First().Athlete.ID);
        }

        [Fact]
        public async Task CanGetBestAverageResults()
        {
            //arrange
            var course = Substitute.For<Course>();
            course.Results = new List<Result>
            {
                new Result { Athlete = new Athlete { ID = 123 }, Duration = TimeSpan.Parse("2:34") },
                new Result { Athlete = new Athlete { ID = 234 }, Duration = TimeSpan.Parse("1:23") },
            };

            var dataService = Substitute.For<IDataService>();
            dataService.GetCourse(Arg.Any<uint>()).Returns(course);

            var controller = new CourseController(dataService);

            //act
            var response = await controller.BestAverage(123);

            //assert
            Assert.Equal((uint)234, ((ResultsViewModel<TimeSpan>)(response.Model)).RankedResults.First().Athlete.ID);
        }

        [Fact]
        public async Task CanGetMostRuns()
        {
            //arrange
            var course = Substitute.For<Course>();
            course.Results = new List<Result>
            {
                new Result { Athlete = new Athlete { ID = 123 }, Duration = TimeSpan.Parse("2:34") },
                new Result { Athlete = new Athlete { ID = 234 }, Duration = TimeSpan.Parse("1:23") },
            };

            var dataService = Substitute.For<IDataService>();
            dataService.GetCourse(Arg.Any<uint>()).Returns(course);

            var controller = new CourseController(dataService);

            //act
            var response = await controller.MostRuns(123);

            //assert
            Assert.Equal((uint)234, ((ResultsViewModel<ushort>)(response.Model)).RankedResults.First().Athlete.ID);
        }
    }
}
