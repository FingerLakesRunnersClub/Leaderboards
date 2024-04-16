using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Tests.Community;
using FLRC.Leaderboards.Core.Tests.Leaders;
using FLRC.Leaderboards.Web.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests;

public sealed class CommunityControllerTests
{
	[Fact]
	public async Task CanShowOutstandingGroupsToAdd()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		dataService.GetAthletes().Returns(LeaderboardData.Athletes);
		dataService.GetCommunityUsers().Returns(CommunityData.Users);
		dataService.GetCommunityGroupMembers("everybody-everybody").Returns([CommunityData.User1, CommunityData.User2, CommunityData.User5]);
		dataService.GetCommunityGroupMembers("youngins").Returns([CommunityData.User1]);
		dataService.GetCommunityGroupMembers("dirty-thirties").Returns([]);
		dataService.GetCommunityGroupMembers("masters").Returns([CommunityData.User5]);
		dataService.GetCommunityGroupMembers("oldies-but-goodies").Returns([]);
		var config = Substitute.For<IConfig>();
		config.CommunityGroups.Returns(CommunityData.Groups);
		var controller = new CommunityController(dataService, config);

		//act
		var result = await controller.Admin();

		//assert
		var vm = result.Model as CommunityAdminViewModel;
		var rows = vm!.Rows;
		Assert.Equal(5, rows.Length);
		Assert.Equal(CommunityData.User1, rows.First(r => r.Athlete.Equals(LeaderboardData.Athlete1)).User);
		Assert.Equal(CommunityData.User2, rows.First(r => r.Athlete.Equals(LeaderboardData.Athlete2)).User);
		Assert.Equal(CommunityData.User3, rows.First(r => r.Athlete.Equals(LeaderboardData.Athlete3)).User);
		Assert.Null(rows.First(r => r.Athlete.Equals(LeaderboardData.Athlete4)).User);
		Assert.Equal(CommunityData.User5, rows.First(r => r.Athlete.Equals(LeaderboardData.Private)).User);
	}

	[Fact]
	public async Task CanAddGroupsToSpecifiedUsers()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		dataService.GetAthletes().Returns(LeaderboardData.Athletes);
		dataService.GetCommunityUsers().Returns(CommunityData.Users);
		dataService.GetCommunityGroupMembers("everybody-everybody").Returns([CommunityData.User1, CommunityData.User2, CommunityData.User5]);
		dataService.GetCommunityGroupMembers("youngins").Returns([CommunityData.User1]);
		dataService.GetCommunityGroupMembers("dirty-thirties").Returns([]);
		dataService.GetCommunityGroupMembers("masters").Returns([CommunityData.User5]);
		dataService.GetCommunityGroupMembers("oldies-but-goodies").Returns([]);

		IDictionary<string, string[]> updates = new Dictionary<string, string[]>();
		await dataService.AddCommunityGroupMembers(Arg.Do<IDictionary<string, string[]>>(d => updates = d));

		var config = Substitute.For<IConfig>();
		config.CommunityGroups.Returns(CommunityData.Groups);
		var controller = new CommunityController(dataService, config);

		//act
		await controller.Admin([12, 23, 34]);

		//assert
		var expected = new Dictionary<string, string[]>
		{
			{ "everybody-everybody", ["u3"] },
			{ "youngins", ["u3"] },
			{ "dirty-thirties", ["u2"] }
		};
		Assert.Equal(expected, updates);
	}
}