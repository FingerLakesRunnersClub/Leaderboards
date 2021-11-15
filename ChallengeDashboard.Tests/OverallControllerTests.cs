using System.Linq;
using System.Threading.Tasks;
using FLRC.ChallengeDashboard.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests;

public class OverallControllerTests
{
	[Fact]
	public async Task CanGetMostPoints()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		dataService.GetAllResults().Returns(LeaderboardData.Courses);
		var controller = new OverallController(dataService);

		//act
		var response = await controller.Points(string.Empty);

		//assert
		var vm = (OverallResultsViewModel<Points>)response.Model;
		Assert.Equal(LeaderboardData.Athlete1, vm.RankedResults.First().Result.Athlete);
	}

	[Fact]
	public async Task CanGetMostMiles()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		dataService.GetAllResults().Returns(LeaderboardData.Courses);
		var controller = new OverallController(dataService);

		//act
		var response = await controller.Miles();

		//assert
		var vm = (OverallResultsViewModel<double>)response.Model;
		Assert.Equal(LeaderboardData.Athlete2, vm.RankedResults.First().Result.Athlete);
	}

	[Fact]
	public async Task CanGetBestAgeGradeAverage()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		dataService.GetAllResults().Returns(LeaderboardData.Courses);
		var controller = new OverallController(dataService);

		//act
		var response = await controller.AgeGrade();

		//assert
		var vm = (OverallResultsViewModel<AgeGrade>)response.Model;
		Assert.Equal(LeaderboardData.Athlete1, vm.RankedResults.First().Result.Athlete);
	}

	[Fact]
	public async Task CanGetTeamPoints()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		dataService.GetAllResults().Returns(LeaderboardData.Courses);
		var controller = new OverallController(dataService);

		//act
		var response = await controller.Team();

		//assert
		var vm = (OverallTeamResultsViewModel)response.Model;
		Assert.Equal("1–29", vm.Results.First().Team.Display);
	}
}
