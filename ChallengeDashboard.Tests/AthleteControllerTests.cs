using System;
using System.Collections.Generic;
using System.Linq;
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
            var athlete = new Athlete {ID = 123, Category = Category.M};
            var course = new Course
            {
                Meters = 10 * Course.MetersPerMile,
                Results = new List<Result>
                {
                    new Result
                    {
                        Athlete = athlete,
                        Duration = new Time(new TimeSpan(2, 4, 6))
                    },
                    new Result
                    {
                        Athlete = new Athlete {ID = 234, Category = Category.F},
                        Duration = new Time(new TimeSpan(1, 2, 3))
                    }
                }
            };
            dataService.GetAthlete(123).Returns(athlete);
            dataService.GetAllResults().Returns(new List<Course> {course});
            var controller = new AthleteController(dataService);

            //act
            var response = await controller.Index(123);

            //assert
            var vm = (AthleteSummaryViewModel) response.Model;
            Assert.Equal((uint) 123, vm.Athlete.ID);
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
                        Athlete = new Athlete {ID = 123},
                        Duration = new Time(new TimeSpan(1, 2, 3))
                    }
                }
            };
            dataService.GetResults(123).Returns(course);
            var controller = new AthleteController(dataService);

            //act
            var response = await controller.Course(123, 123);

            //assert
            var vm = (AthleteCourseResultsViewModel) response.Model;
            Assert.NotEmpty(vm.Results);
        }

        [Fact]
        public async Task CanFindSimilarAthletes()
        {
            //arrange
            var course = new Course {Results = CourseData.Results};
            var dataService = Substitute.For<IDataService>();
            dataService.GetAthlete(123).Returns(CourseData.Athlete1);
            dataService.GetAthlete(234).Returns(CourseData.Athlete2);
            dataService.GetAthlete(345).Returns(CourseData.Athlete3);
            dataService.GetAthlete(456).Returns(CourseData.Athlete4);
            dataService.GetAllResults().Returns(new[] {course});

            var controller = new AthleteController(dataService);

            //act
            var result = await controller.Similar(123);

            //assert
            var vm = (SimilarAthletesViewModel) result.Model;
            var match = vm.Matches.Single();
            Assert.Equal(CourseData.Athlete1, vm.Athlete);
            Assert.Equal(CourseData.Athlete4, match.Athlete);
            Assert.Equal("87%", match.Similarity.Display);
        }
    }
}