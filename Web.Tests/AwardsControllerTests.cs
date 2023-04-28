using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Reports;
using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Core.Tests.Leaders;
using FLRC.Leaderboards.Web.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests;

public sealed class AwardsControllerTests
{
	[Fact]
	public async Task CalculatesCorrectAwards()
	{
		//arrange
		var results = LeaderboardData.Courses;
		var dataService = Substitute.For<IDataService>();
		dataService.GetAllResults().Returns(results);
		var controller = new AwardsController(dataService, TestHelpers.Config);

		//act
		var response = await controller.Index();

		//assert
		var vm = (AwardsViewModel) response.Model!;
		var athletes = vm.Awards.Count;
		var count = vm.Awards.Sum(athlete => athlete.Value.Length);
		var amount = vm.Awards.Sum(athlete => athlete.Value.Sum(award => award.Value));
		Assert.Equal(4, athletes);
		Assert.Equal(27, count);
		Assert.Equal(91, amount);
		Assert.Contains("1st Overall Points (M)", vm.Awards[LeaderboardData.Athlete1].Select(a => a.Name));
		Assert.Contains("2nd Overall Community", vm.Awards[LeaderboardData.Athlete1].Select(a => a.Name));
		Assert.Contains("2nd Overall Points (M)", vm.Awards[LeaderboardData.Athlete2].Select(a => a.Name));
		Assert.Contains("1st Overall Miles", vm.Awards[LeaderboardData.Athlete2].Select(a => a.Name));
		Assert.Contains("1st Overall Points (F)", vm.Awards[LeaderboardData.Athlete3].Select(a => a.Name));
		Assert.Contains("1st Overall Community", vm.Awards[LeaderboardData.Athlete4].Select(a => a.Name));
		Assert.Contains("2nd Overall Points (F)", vm.Awards[LeaderboardData.Athlete4].Select(a => a.Name));
		Assert.Contains("1st Overall Miles", vm.Awards[LeaderboardData.Athlete4].Select(a => a.Name));
	}
}