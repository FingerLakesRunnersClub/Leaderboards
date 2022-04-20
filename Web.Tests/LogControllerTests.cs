using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Reports;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Web.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web;

public class LogControllerTests
{
	[Fact]
	public async Task RecentShowsResultsInReverseChronologicalOrder()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var course = new Course { Results = CourseData.Results };
		dataService.GetAllResults().Returns(new[] { course });
		var controller = new LogController(dataService, TestHelpers.Config, new DateTime(2020, 1, 8));

		//act
		var response = await controller.Index();

		//assert
		var vm = (ActivityLogViewModel) response.Model;
		var results = vm!.Results.SelectMany(g => g).ToArray();
		Assert.Equal(7, results.Length);
		Assert.Equal(new DateTime(2020, 1, 8), results[0].StartTime.Value);
		Assert.Equal(new DateTime(2020, 1, 7), results[1].StartTime.Value);
		Assert.Equal(new DateTime(2020, 1, 6), results[2].StartTime.Value);
		Assert.Equal(new DateTime(2020, 1, 5), results[3].StartTime.Value);
		Assert.Equal(new DateTime(2020, 1, 4), results[4].StartTime.Value);
		Assert.Equal(new DateTime(2020, 1, 3), results[5].StartTime.Value);
		Assert.Equal(new DateTime(2020, 1, 2), results[6].StartTime.Value);
	}

	[Fact]
	public async Task ArchiveLogShowsResultsInReverseChronologicalOrder()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var course = new Course { Results = CourseData.Results };
		dataService.GetAllResults().Returns(new[] { course });
		var controller = new LogController(dataService, TestHelpers.Config, new DateTime(2020, 1, 8));

		//act
		var response = await controller.All();

		//assert
		var vm = (ActivityLogViewModel) response.Model;
		var results = vm!.Results.SelectMany(g => g).ToArray();
		Assert.Equal(new DateTime(2020, 1, 8), results[0].StartTime.Value);
		Assert.Equal(new DateTime(2020, 1, 7), results[1].StartTime.Value);
		Assert.Equal(new DateTime(2020, 1, 6), results[2].StartTime.Value);
		Assert.Equal(new DateTime(2020, 1, 5), results[3].StartTime.Value);
		Assert.Equal(new DateTime(2020, 1, 4), results[4].StartTime.Value);
		Assert.Equal(new DateTime(2020, 1, 3), results[5].StartTime.Value);
		Assert.Equal(new DateTime(2020, 1, 2), results[6].StartTime.Value);
		Assert.Equal(new DateTime(2020, 1, 1), results[7].StartTime.Value);
	}

	[Fact]
	public async Task SameMonthsInDifferentYearsAreSeparated()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var results = new List<Result>();
		results.AddRange(CourseData.Results);
		results.Add(new Result { StartTime = new Date(new DateTime(2021, 1, 1)) });
		var course = new Course { Results = results };
		dataService.GetAllResults().Returns(new[] { course });
		var controller = new LogController(dataService, TestHelpers.Config, new DateTime(2020, 1, 8));

		//act
		var response = await controller.All();

		//assert
		var vm = (ActivityLogViewModel) response.Model;
		var groups = vm!.Results.Select(g => g.Key).ToArray();
		Assert.Equal(2, groups.Length);
		Assert.Equal("January 2021", groups[0]);
		Assert.Equal("January 2020", groups[1]);
	}

	[Fact]
	public async Task CanGetActivityLogForCourse()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var course = new Course { Results = CourseData.Results };
		dataService.GetResults(123, null).Returns(course);
		var controller = new LogController(dataService, TestHelpers.Config, new DateTime(2020, 1, 8));

		//act
		var response = await controller.Index(123);

		//assert
		var vm = (ActivityLogViewModel) response.Model;
		var results = vm!.Results.SelectMany(g => g).ToArray();
		Assert.Equal(7, results.Length);
		Assert.Equal(new DateTime(2020, 1, 8), results[0].StartTime.Value);
		Assert.Equal(new DateTime(2020, 1, 7), results[1].StartTime.Value);
		Assert.Equal(new DateTime(2020, 1, 6), results[2].StartTime.Value);
		Assert.Equal(new DateTime(2020, 1, 5), results[3].StartTime.Value);
		Assert.Equal(new DateTime(2020, 1, 4), results[4].StartTime.Value);
		Assert.Equal(new DateTime(2020, 1, 3), results[5].StartTime.Value);
		Assert.Equal(new DateTime(2020, 1, 2), results[6].StartTime.Value);
	}
}