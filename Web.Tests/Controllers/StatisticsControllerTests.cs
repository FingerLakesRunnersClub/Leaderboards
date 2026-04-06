using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Controllers;
using FLRC.Leaderboards.Web.ViewModels;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Controllers;

public sealed class StatisticsControllerTests
{
	[Fact]
	public async Task CanGetAllStatistics()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var course = new Course { Results = ResultsData.Results };
		var iteration = new Iteration
		{
			StartDate = new DateOnly(2020, 1, 1),
			EndDate = new DateOnly(2020, 12, 31),
			Challenges = [new Challenge { IsOfficial = true, IsPrimary = true, Courses = [course] }]
		};
		iterationManager.ActiveIteration().Returns(iteration);
		var controller = new StatisticsController(iterationManager);

		//act
		var response = await controller.Index();

		//assert
		var vm = response.Model as ViewModel<StatisticsViewModel>;
		var courseStats = vm!.Data.Courses[course];
		Assert.Equal(4, courseStats.Participants[string.Empty]);
		Assert.Equal(8, courseStats.Runs[string.Empty]);
		Assert.Equal(8 * 10000 / Core.Races.Distance.MetersPerMile, courseStats.Miles[string.Empty]);
		Assert.Equal(2, courseStats.Average[string.Empty]);

		var weeklyStats = vm.Data.History.ToArray();
		Assert.Equal(new DateOnly(2020, 2, 8), weeklyStats[0].Key);
		Assert.Equal(1, weeklyStats[0].Value.Runs[string.Empty]);
		Assert.Equal(new DateOnly(2020, 2, 1), weeklyStats[1].Key);
		Assert.Equal(7, weeklyStats[1].Value.Runs[string.Empty]);
	}
}