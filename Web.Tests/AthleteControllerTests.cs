using FLRC.Leaderboards.Core;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Web.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web;

public class AthleteControllerTests
{
	[Fact]
	public async Task PageContainsAllInfo()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var athlete = new Athlete { ID = 123, Category = Category.M };
		var course = new Course
		{
			Meters = 10 * Course.MetersPerMile,
			Results = new List<Result>
				{
					new Result
					{
						Athlete = athlete,
						Course = CourseData.Course,
						Duration = new Time(new TimeSpan(2, 4, 6))
					},
					new Result
					{
						Athlete = new Athlete {ID = 234, Category = Category.F},
						Course = CourseData.Course,
						Duration = new Time(new TimeSpan(1, 2, 3))
					}
				}
		};
		dataService.GetAthlete(123).Returns(athlete);
		dataService.GetAllResults().Returns(new List<Course> { course });
		var controller = new AthleteController(dataService);

		//act
		var response = await controller.Index(123);

		//assert
		var vm = (AthleteSummaryViewModel)response.Model;
		Assert.Equal((uint)123, vm.Summary.Athlete.ID);
		Assert.Equal(1, vm.Summary.Fastest[course].Rank.Value);
		Assert.Equal(1, vm.Summary.Average[course].Rank.Value);
		Assert.Equal(1, vm.Summary.Runs[course].Rank.Value);
		Assert.Equal(1, vm.Summary.TeamResults.Rank.Value);
		Assert.Equal(100, vm.Summary.OverallPoints.Value.Value);
		Assert.Equal(10, vm.Summary.OverallMiles.Value);
	}

	[Fact]
	public async Task CanGetAllResultsForActivityLog()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var athlete = new Athlete { ID = 123 };
		var course = new Course
		{
			ID = 234,
			Meters = 10 * Course.MetersPerMile,
			Results = new List<Result>
				{
					new()
					{
						Athlete = athlete,
						Course = CourseData.Course,
						Duration = new Time(new TimeSpan(1, 2, 3))
					}
				}
		};
		dataService.GetAthlete(123).Returns(athlete);
		dataService.GetAllResults().Returns(new[] { course });
		var controller = new AthleteController(dataService);

		//act
		var response = await controller.Log(123);

		//assert
		var vm = (AthleteLogViewModel)response.Model;
		Assert.NotEmpty(vm.Results);
	}

	[Fact]
	public async Task CanGetAllResultsForCourse()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var course = new Course
		{
			ID = 234,
			Meters = 10 * Course.MetersPerMile,
			Results = new List<Result>
				{
					new Result
					{
						Athlete = new Athlete {ID = 123},
						Course = CourseData.Course,
						Duration = new Time(new TimeSpan(1, 2, 3))
					}
				}
		};
		dataService.GetResults(234).Returns(course);
		var controller = new AthleteController(dataService);

		//act
		var response = await controller.Course(123, 234);

		//assert
		var vm = (AthleteCourseResultsViewModel)response.Model;
		Assert.NotEmpty(vm.Results);
	}

	[Fact]
	public async Task ResultsAreRankedCorrectly()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var course = new Course
		{
			ID = 234,
			Meters = 10 * Course.MetersPerMile,
			Results = new List<Result>
				{
					new()
					{
						Athlete = new Athlete {ID = 123, Age = 35 },
						Course = CourseData.Course,
						Duration = new Time(new TimeSpan(0, 2, 3))
					},
					new()
					{
						Athlete = new Athlete {ID = 123, Age = 35},
						Course = CourseData.Course,
						Duration = new Time(new TimeSpan(1, 2, 3))
					},
					new()
					{
						Athlete = new Athlete {ID = 123, Age = 35},
						Course = CourseData.Course,
						Duration = new Time(new TimeSpan(2, 3, 4))
					},
					new()
					{
						Athlete = new Athlete {ID = 123, Age = 35},
						Course = CourseData.Course,
						Duration = new Time(new TimeSpan(0, 2, 3))
					},
					new()
					{
						Athlete = new Athlete {ID = 123, Age = 35},
						Course = CourseData.Course,
						Duration = new Time(new TimeSpan(1, 2, 3))
					}
				}
		};
		dataService.GetResults(234).Returns(course);
		var controller = new AthleteController(dataService);

		//act
		var response = await controller.Course(123, 234);

		//assert
		var vm = (AthleteCourseResultsViewModel)response.Model;
		Assert.Equal(0, vm.Results[0].Rank.Value);
		Assert.Equal(0, vm.Results[1].Rank.Value);
		Assert.Equal(1, vm.Results[2].Rank.Value);
		Assert.Equal(1, vm.Results[3].Rank.Value);
		Assert.Equal(3, vm.Results[4].Rank.Value);
	}

	[Fact]
	public async Task CanFindSimilarAthletes()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		dataService.GetAthlete(123).Returns(CourseData.Athlete1);
		dataService.GetAthlete(234).Returns(CourseData.Athlete2);
		dataService.GetAthlete(345).Returns(CourseData.Athlete3);
		dataService.GetAthlete(456).Returns(CourseData.Athlete4);
		dataService.GetAllResults().Returns(new[] { new Course { Results = CourseData.SimilarResults } });

		var controller = new AthleteController(dataService);

		//act
		var result = await controller.Similar(123);

		//assert
		var vm = (SimilarAthletesViewModel)result.Model;
		var matches = vm.Matches.ToList();
		Assert.Equal(CourseData.Athlete1, vm.Athlete);
		Assert.Equal(CourseData.Athlete4, matches[0].Athlete);
		Assert.Equal("96%", matches[0].Similarity.Display);
		Assert.Equal(CourseData.Athlete2, matches[1].Athlete);
		Assert.Equal("95%", matches[1].Similarity.Display);
	}
}