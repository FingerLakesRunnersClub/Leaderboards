using FLRC.Leaderboards.Core;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Reports;
using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Web.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web;

public class StatisticsControllerTests
{
	[Fact]
	public async Task CanGetAllStatistics()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var course = new Course { Results = CourseData.Results, Meters = 10000 };
		dataService.GetAllResults().Returns(new[] { course });
		var controller = new StatisticsController(dataService);

		//act
		var response = await controller.Index();

		//assert
		var vm = (StatisticsViewModel)response.Model;
		var courseStats = vm.Courses[course];
		Assert.Equal(4, courseStats.Participants[string.Empty]);
		Assert.Equal(8, courseStats.Runs[string.Empty]);
		Assert.Equal(8 * 10000 / Course.MetersPerMile, courseStats.Miles[string.Empty]);
		Assert.Equal(2, courseStats.Average[string.Empty]);

		var weeklyStats = vm.History.ToArray();
		Assert.Equal(new DateTime(2020, 1, 2), weeklyStats[0].Key);
		Assert.Equal(7, weeklyStats[0].Value.Runs[string.Empty]);
		Assert.Equal(new DateTime(2019, 12, 26), weeklyStats[1].Key);
		Assert.Equal(1, weeklyStats[1].Value.Runs[string.Empty]);
	}
}