using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FLRC.ChallengeDashboard.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class AthleteControllerTests
    {
        [Fact]
        public async Task PageContainsAllInfo()
        {
            //arrange
            var dataService = Substitute.For<IDataService>();
            var course = new Course
            {
                Meters = 10 * Course.MetersPerMile,
                Results = new List<Result>
                    {
                        new Result
                        {
                            Athlete = new Athlete { ID = 123 },
                            Duration = new Time(new TimeSpan(1, 2, 3))
                        }
                    }
            };
            dataService.GetAllResults().Returns(new List<Course> { course });
            var controller = new AthleteController(dataService);

            //act
            var response = await controller.Index(123);

            //assert
            var vm = (AthleteSummaryViewModel)(response.Model);
            Assert.Equal((uint)123, vm.Athlete.ID);
            Assert.Equal(1, vm.Fastest[course].Rank.Value);
            Assert.Equal(1, vm.Average[course].Rank.Value);
            Assert.Equal(1, vm.Runs[course].Rank.Value);
            Assert.Equal(1, vm.TeamResults.Rank.Value);
            Assert.Equal(100, vm.OverallPoints.Value.Value);
            Assert.Equal(10, vm.OverallMiles.Value);
        }

        [Fact]
        public async Task CanGetAllResultsForCourse()
        {
            //arrange
            var dataService = Substitute.For<IDataService>();
            var course = new Course
            {
                ID = 123,
                Meters = 10 * Course.MetersPerMile,
                Results = new List<Result>
                {
                    new Result
                    {
                        Athlete = new Athlete { ID = 123 },
                        Duration = new Time(new TimeSpan(1, 2, 3))
                    }
                }
            };
            dataService.GetResults(123).Returns(course);
            var controller = new AthleteController(dataService);
            
            //act
            var response = await controller.Course(123, 123);
            
            //assert
            var vm = (AthleteCourseViewModel) response.Model;
            Assert.NotEmpty(vm.Results);
        }
    }
}