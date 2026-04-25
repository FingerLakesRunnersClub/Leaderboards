using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Tests.Community;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Areas.Admin.Controllers;
using FLRC.Leaderboards.Web.Areas.Admin.ViewModels;
using FLRC.Leaderboards.Web.ViewModels;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Admin;

public sealed class CommunityControllerTests
{
	[Fact]
	public async Task CanShowOutstandingGroupsToAdd()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var communityManager = Substitute.For<ICommunityManager>();
		var config = Substitute.For<IConfig>();
		var controller = new CommunityController(iterationManager, communityManager, config);

		iterationManager.ActiveIteration().Returns(CommunityData.Iteration);

		communityManager.GetCommunityUsers().Returns(CommunityData.Users);
		communityManager.GetCommunityGroupMembers("everybody-everybody").Returns([CommunityData.User1, CommunityData.User2, CommunityData.User5]);
		communityManager.GetCommunityGroupMembers("youngins").Returns([CommunityData.User1]);
		communityManager.GetCommunityGroupMembers("dirty-thirties").Returns([]);
		communityManager.GetCommunityGroupMembers("masters").Returns([CommunityData.User5]);
		communityManager.GetCommunityGroupMembers("oldies-but-goodies").Returns([]);

		config.CommunityGroups.Returns(CommunityData.Groups);

		//act
		var result = await controller.Index();

		//assert
		var vm = result.Model as ViewModel<CommunityAdmin>;
		var rows = vm!.Data.MissingRows.Union(vm!.Data.NoUserRows).Union(vm!.Data.SyncedRows).ToArray();
		Assert.Equal(5, rows.Length);
		Assert.Equal(CommunityData.User1, rows.First(r => r.Athlete.Equals(CommunityData.Athlete1)).User);
		Assert.Equal(CommunityData.User2, rows.First(r => r.Athlete.Equals(CommunityData.Athlete2)).User);
		Assert.Equal(CommunityData.User3, rows.First(r => r.Athlete.Equals(CommunityData.Athlete3)).User);
		Assert.Null(rows.First(r => r.Athlete.Equals(CommunityData.Athlete4)).User);
		Assert.Equal(CommunityData.User5, rows.First(r => r.Athlete.Equals(CommunityData.Private)).User);
	}
}