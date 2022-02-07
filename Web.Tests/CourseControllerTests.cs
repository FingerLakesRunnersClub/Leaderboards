using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Web.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web;

public class CourseControllerTests
{
	[Fact]
	public async Task ResultsGetCourseInfo()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		dataService.GetResults(Arg.Any<uint>()).Returns(new Course { Results = new List<Result>() });
		var controller = new CourseController(dataService, TestHelpers.Config);

		//act
		await controller.Fastest(123);

		//assert
		await dataService.Received().GetResults(123);
	}

	[Fact]
	public async Task CanGetFastestResults()
	{
		//arrange
		var course = new Course
		{
			Distance = new Distance("10K"),
			Results = new List<Result>
			{
				new() { Athlete = new Athlete { ID = 123 }, Duration = new Time(TimeSpan.Parse("2:34")) },
				new() { Athlete = new Athlete { ID = 234 }, Duration = new Time(TimeSpan.Parse("1:23")) },
			}
		};

		var dataService = Substitute.For<IDataService>();
		dataService.GetResults(Arg.Any<uint>()).Returns(course);

		var controller = new CourseController(dataService, TestHelpers.Config);

		//act
		var response = await controller.Fastest(123);

		//assert
		Assert.Equal((uint) 234, ((CourseResultsViewModel<Time>) response.Model!).RankedResults.First().Result.Athlete.ID);
	}

	[Fact]
	public async Task CanGetBestAverageResults()
	{
		//arrange
		var course = new Course
		{
			Distance = new Distance("10K"),
			Results = new List<Result>
			{
				new() { Athlete = new Athlete { ID = 123 }, Duration = new Time(TimeSpan.Parse("2:34")) },
				new() { Athlete = new Athlete { ID = 234 }, Duration = new Time(TimeSpan.Parse("1:23")) },
			}
		};

		var dataService = Substitute.For<IDataService>();
		dataService.GetResults(Arg.Any<uint>()).Returns(course);

		var controller = new CourseController(dataService, TestHelpers.Config);

		//act
		var response = await controller.BestAverage(123);

		//assert
		Assert.Equal((uint) 234, ((CourseResultsViewModel<Time>) response.Model!).RankedResults.First().Result.Athlete.ID);
	}

	[Fact]
	public async Task CanGetMostRuns()
	{
		//arrange
		var course = new Course
		{
			Distance = new Distance("10K"),
			Results = new List<Result>
			{
				new() { Athlete = new Athlete { ID = 123 }, Duration = new Time(TimeSpan.Parse("2:34")) },
				new() { Athlete = new Athlete { ID = 234 }, Duration = new Time(TimeSpan.Parse("1:23")) },
			}
		};

		var dataService = Substitute.For<IDataService>();
		dataService.GetResults(Arg.Any<uint>()).Returns(course);

		var controller = new CourseController(dataService, TestHelpers.Config);

		//act
		var response = await controller.MostRuns(123);

		//assert
		Assert.Equal((uint) 234, ((CourseResultsViewModel<ushort>) response.Model!).RankedResults.First().Result.Athlete.ID);
	}

	[Fact]
	public async Task CanGetTeamResults()
	{
		//arrange
		var course = new Course
		{
			Distance = new Distance("10 miles"),
			Results = new List<Result>
			{
				new Result { Athlete = new Athlete { ID = 123, Age = 20 }, Duration = new Time(TimeSpan.Parse("2:34")) },
				new Result { Athlete = new Athlete { ID = 123, Age = 20 }, Duration = new Time(TimeSpan.Parse("2:35")) },
				new Result { Athlete = new Athlete { ID = 234, Age = 30 }, Duration = new Time(TimeSpan.Parse("1:23")) },
			}
		};

		var dataService = Substitute.For<IDataService>();
		dataService.GetResults(Arg.Any<uint>()).Returns(course);

		var controller = new CourseController(dataService, TestHelpers.Config);

		//act
		var response = await controller.Team(123);

		//assert
		var vm = (CourseTeamResultsViewModel) response.Model;
		var results = vm!.Results.ToList();
		var team2 = results.First(r => r.Team == Athlete.Teams[2]);
		var team3 = results.First(r => r.Team == Athlete.Teams[3]);
		Assert.Equal(1, team3.AgeGradePoints);
		Assert.Equal(2, team2.AgeGradePoints);
		Assert.Equal(1, team2.MostRunsPoints);
		Assert.Equal(2, team3.MostRunsPoints);
		Assert.Equal(3, team2.TotalPoints);
		Assert.Equal(3, team3.TotalPoints);
	}
}