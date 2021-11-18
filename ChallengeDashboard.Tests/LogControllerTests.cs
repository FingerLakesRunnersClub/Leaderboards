using System;
using System.Linq;
using System.Threading.Tasks;
using FLRC.ChallengeDashboard.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests;

public class LogControllerTests
{
	[Fact]
	public async Task RecentShowsResultsInReverseChronologicalOrder()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var course = new Course { Results = CourseData.Results };
		dataService.GetAllResults().Returns(new[] { course });
		var controller = new LogController(dataService, new DateTime(2020, 1, 8));

		//act
		var response = await controller.Index();

		//assert
		var vm = (ActivityLogViewModel)response.Model;
		var results = vm.Results.SelectMany(g => g).ToArray();
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
		var controller = new LogController(dataService, new DateTime(2020, 1, 8));

		//act
		var response = await controller.All();

		//assert
		var vm = (ActivityLogViewModel)response.Model;
		var results = vm.Results.SelectMany(g => g).ToArray();
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
	public async Task CanGetActivityLogForCourse()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var course = new Course { Results = CourseData.Results };
		dataService.GetResults(123).Returns(course);
		var controller = new LogController(dataService, new DateTime(2020, 1, 8));

		//act
		var response = await controller.Index(123);

		//assert
		var vm = (ActivityLogViewModel)response.Model;
		var results = vm.Results.SelectMany(g => g).ToArray();
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