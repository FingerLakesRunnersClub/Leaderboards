using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Web.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests;

public class CourseControllerTests
{
	[Fact]
	public async Task ResultsGetCourseInfo()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		dataService.GetResults(Arg.Any<uint>(), Arg.Any<string>()).Returns(new Course { Results = Array.Empty<Result>() });
		var controller = new CourseController(dataService, TestHelpers.Config);

		//act
		await controller.Fastest(123, null);

		//assert
		await dataService.Received().GetResults(123, null);
	}

	[Fact]
	public async Task CanGetFastestResults()
	{
		//arrange
		var results = new List<Result>();
		var course = new Course
		{
			Distance = new Distance("10K"),
			Results = results
		};
		results.AddRange(new []
		{
			new Result { Course = course, Athlete = new Athlete { ID = 123 }, StartTime = new Date(new DateTime(2022, 4, 26)), Duration = new Time(TimeSpan.Parse("2:34")) },
			new Result { Course = course, Athlete = new Athlete { ID = 234 }, StartTime = new Date(new DateTime(2022, 4, 26)), Duration = new Time(TimeSpan.Parse("1:23")) },
		});

		var dataService = Substitute.For<IDataService>();
		dataService.GetResults(Arg.Any<uint>(), Arg.Any<string>()).Returns(course);

		var controller = new CourseController(dataService, TestHelpers.Config);

		//act
		var response = await controller.Fastest(123, null);

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
			Results = new []
			{
				new Result { Athlete = new Athlete { ID = 123 }, StartTime = new Date(new DateTime(2022, 4, 26)), Duration = new Time(TimeSpan.Parse("2:34")) },
				new Result { Athlete = new Athlete { ID = 234 }, StartTime = new Date(new DateTime(2022, 4, 26)), Duration = new Time(TimeSpan.Parse("1:23")) },
			}
		};

		var dataService = Substitute.For<IDataService>();
		dataService.GetResults(Arg.Any<uint>(), Arg.Any<string>()).Returns(course);

		var controller = new CourseController(dataService, TestHelpers.Config);

		//act
		var response = await controller.BestAverage(123, null);

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
			Results = new []
			{
				new Result { Athlete = new Athlete { ID = 123 }, StartTime = new Date(new DateTime(2022, 4, 26)), Duration = new Time(TimeSpan.Parse("2:34")) },
				new Result { Athlete = new Athlete { ID = 234 }, StartTime = new Date(new DateTime(2022, 4, 26)), Duration = new Time(TimeSpan.Parse("1:23")) },
			}
		};

		var dataService = Substitute.For<IDataService>();
		dataService.GetResults(Arg.Any<uint>(), Arg.Any<string>()).Returns(course);

		var controller = new CourseController(dataService, TestHelpers.Config);

		//act
		var response = await controller.MostRuns(123, null);

		//assert
		Assert.Equal((uint) 234, ((CourseResultsViewModel<ushort>) response.Model!).RankedResults.First().Result.Athlete.ID);
	}

	[Fact]
	public async Task CanGetTeamResults()
	{
		//arrange
		var results = new List<Result>();
		var course = new Course
		{
			Distance = new Distance("10 miles"),
			Results = results
		};
		results.AddRange(new[]
		{
			new Result { Course = course, Athlete = new Athlete { ID = 123, Age = 20 }, StartTime = new Date(new DateTime(2022, 4, 26)), Duration = new Time(TimeSpan.Parse("2:34")) },
			new Result { Course = course, Athlete = new Athlete { ID = 123, Age = 20 }, StartTime = new Date(new DateTime(2022, 4, 26)), Duration = new Time(TimeSpan.Parse("2:35")) },
			new Result { Course = course, Athlete = new Athlete { ID = 234, Age = 30 }, StartTime = new Date(new DateTime(2022, 4, 26)), Duration = new Time(TimeSpan.Parse("1:23")) },
		});

		var dataService = Substitute.For<IDataService>();
		dataService.GetResults(Arg.Any<uint>(), Arg.Any<string>()).Returns(course);

		var controller = new CourseController(dataService, TestHelpers.Config);

		//act
		var response = await controller.Team(123, null);

		//assert
		var vm = (CourseResultsViewModel<TeamResults>) response.Model;
		var ranked = vm!.RankedResults.ToArray();
		var team2 = ranked.First(r => r.Value.Team == Athlete.Teams[2]);
		var team3 = ranked.First(r => r.Value.Team == Athlete.Teams[3]);
		Assert.Equal(1, team3.Value.AgeGradePoints);
		Assert.Equal(2, team2.Value.AgeGradePoints);
		Assert.Equal(1, team2.Value.MostRunsPoints);
		Assert.Equal(2, team3.Value.MostRunsPoints);
		Assert.Equal(3, team2.Value.TotalPoints);
		Assert.Equal(3, team3.Value.TotalPoints);
	}

	[Fact]
	public async Task CanGetCommunityStars()
	{
		//arrange
		var course = new Course
		{
			Distance = new Distance("10K"),
			Results = new []
			{
				new Result { Athlete = new Athlete { ID = 123 }, StartTime = new Date(new DateTime(2022, 4, 26)), Duration = new Time(TimeSpan.Parse("2:34")), CommunityStars = { [StarType.Story] = true, [StarType.GroupRun] = true } },
				new Result { Athlete = new Athlete { ID = 234 }, StartTime = new Date(new DateTime(2022, 4, 26)), Duration = new Time(TimeSpan.Parse("1:23")), CommunityStars = { [StarType.ShopLocal] = true } },
			}
		};

		var dataService = Substitute.For<IDataService>();
		dataService.GetResults(Arg.Any<uint>(), Arg.Any<string>()).Returns(course);

		var controller = new CourseController(dataService, TestHelpers.Config);

		//act
		var response = await controller.Community(123, null);

		//assert
		Assert.Equal((uint) 123, ((CourseResultsViewModel<Stars>) response.Model!).RankedResults.First().Result.Athlete.ID);
	}
}