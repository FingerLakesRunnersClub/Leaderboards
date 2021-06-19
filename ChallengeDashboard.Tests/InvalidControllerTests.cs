using System;
using System.Linq;
using System.Threading.Tasks;
using FLRC.ChallengeDashboard.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class InvalidControllerTests
    {
        [Fact]
        public async Task InvalidResultsAreEmptyWhenAllResultsAreGood()
        {
            //arrange
            var dataService = Substitute.For<IDataService>();
            var course = new Course
            {
                Meters = 10000,
                Results = CourseData.Results
            };
            dataService.GetAllResults().Returns(new[] {course});
            var controller = new InvalidController(dataService);

            //act
            var result = await controller.Index();

            //assert
            var model = (InvalidViewModel) result.Model;
            Assert.Empty(model.Results);
        }

        [Fact]
        public async Task InvalidResultsExistWhenTooFast()
        {
            //arrange
            var dataService = Substitute.For<IDataService>();
            var badResult = new Result
            {
                Athlete = CourseData.Athlete1,
                StartTime = new Date(new DateTime(2021, 7, 1)),
                Duration = new Time(TimeSpan.FromMinutes(5))
            };
            var results = CourseData.Results.ToList();
            results.Add(badResult);
            var course = new Course
            {
                Meters = 10000,
                Results = results
            };
            dataService.GetAllResults().Returns(new[] {course});
            var controller = new InvalidController(dataService);

            //act
            var result = await controller.Index();

            //assert
            var model = (InvalidViewModel) result.Model;
            Assert.Equal(1, model.Results.Count);
        }
    }
}