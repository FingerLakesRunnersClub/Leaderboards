using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Areas.Admin.Controllers;
using FLRC.Leaderboards.Web.Areas.Admin.ViewModels;
using FLRC.Leaderboards.Web.ViewModels;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Admin;

public sealed class AthleteControllerTests
{
	[Fact]
	public async Task CanViewListOfAthletes()
	{
		//arrange
		var adminService = Substitute.For<IAdminService>();
		var athleteService = Substitute.For<IAthleteService>();
		var controller = new AthletesController(adminService, athleteService);

		athleteService.All().Returns([new Athlete(), new Athlete()]);

		//act
		var results = await controller.Index();

		//assert
		var vm = results.Model as ViewModel<Athlete[]>;
		Assert.Equal(2, vm!.Data.Length);
	}

	[Fact]
	public async Task CanViewAthleteForm()
	{
		//arrange
		var adminService = Substitute.For<IAdminService>();
		var athleteService = Substitute.For<IAthleteService>();
		var controller = new AthletesController(adminService, athleteService);

		var id = Guid.NewGuid();
		athleteService.Get(id).Returns(new Athlete());

		//act
		var results = await controller.Edit(id);

		//assert
		Assert.Equal("Form", results.ViewName);
	}

	[Fact]
	public async Task CanEditAthlete()
	{
		//arrange
		var adminService = Substitute.For<IAdminService>();
		var athleteService = Substitute.For<IAthleteService>();
		var controller = new AthletesController(adminService, athleteService);

		var id = Guid.NewGuid();
		athleteService.Get(id).Returns(new Athlete());

		//act
		await controller.Edit(id, new Athlete());

		//assert
		await athleteService.Received().Update(Arg.Any<Athlete>(), Arg.Any<Athlete>());
	}

	[Fact]
	public async Task ToggleAdminAddsAdminWhenNotSet()
	{
		//arrange
		var adminService = Substitute.For<IAdminService>();
		var athleteService = Substitute.For<IAthleteService>();
		var controller = new AthletesController(adminService, athleteService);

		var id = Guid.NewGuid();

		//act
		await controller.ToggleAdmin(id);

		//assert
		await adminService.Received().Add(Arg.Any<Model.Admin>());
	}

	[Fact]
	public async Task ToggleAdminRemovesAdminWhenAlreadySet()
	{
		//arrange
		var adminService = Substitute.For<IAdminService>();
		var athleteService = Substitute.For<IAthleteService>();
		var controller = new AthletesController(adminService, athleteService);

		var id = Guid.NewGuid();
		adminService.Verify(id).Returns(true);

		//act
		await controller.ToggleAdmin(id);

		//assert
		await adminService.Received().Delete(Arg.Any<Model.Admin>());
	}

	[Fact]
	public async Task CanViewMergeForm()
	{
		//arrange
		var adminService = Substitute.For<IAdminService>();
		var athleteService = Substitute.For<IAthleteService>();
		var controller = new AthletesController(adminService, athleteService);

		var id = Guid.NewGuid();
		athleteService.Get(id).Returns(new Athlete());
		athleteService.All().Returns([new Athlete(), new Athlete()]);

		//act
		var result = await controller.Merge(id);

		//assert
		Assert.IsType<ViewModel<MergeAthletesForm>>(result.Model);
	}

	[Fact]
	public async Task MergeRequiresCurrentAthleteToBeSet()
	{
		//arrange
		var adminService = Substitute.For<IAdminService>();
		var athleteService = Substitute.For<IAthleteService>();
		var controller = new AthletesController(adminService, athleteService);

		//act
		var task = async () => await controller.Merge(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

		//assert
		await Assert.ThrowsAsync<ArgumentException>(task);
	}

	[Fact]
	public async Task MergeRequiresDifferentAthletesToBeSet()
	{
		//arrange
		var adminService = Substitute.For<IAdminService>();
		var athleteService = Substitute.For<IAthleteService>();
		var controller = new AthletesController(adminService, athleteService);

		//act
		var id = Guid.NewGuid();
		var task = async () => await controller.Merge(id, id, id);

		//assert
		await Assert.ThrowsAsync<ArgumentException>(task);
	}

	[Fact]
	public async Task MergePerformsAllExpectedActions()
	{
		//arrange
		var adminService = Substitute.For<IAdminService>();
		var athleteService = Substitute.For<IAthleteService>();
		var controller = new AthletesController(adminService, athleteService);

		var id1 = Guid.NewGuid();
		var id2 = Guid.NewGuid();
		var a1 = new Athlete { ID = id1 };
		var a2 = new Athlete { ID = id2 };
		athleteService.Get(id1).Returns(a1);
		athleteService.Get(id2).Returns(a2);

		//act
		await controller.Merge(id1, id1, id2);

		//assert
		await athleteService.Received().MigrateResults(a1, a2);
		await athleteService.Received().MigrateRegistrations(a1, a2);
		await athleteService.Received().MigrateLinkedAccounts(a1, a2);
		await adminService.DidNotReceive().Delete(Arg.Any<Model.Admin>());
		await athleteService.Received().Delete(a1);
	}

	[Fact]
	public async Task MergePerformsAdminDeleteIfAdmin()
	{
		//arrange
		var adminService = Substitute.For<IAdminService>();
		var athleteService = Substitute.For<IAthleteService>();
		var controller = new AthletesController(adminService, athleteService);

		var id1 = Guid.NewGuid();
		var id2 = Guid.NewGuid();
		var a1 = new Athlete { ID = id1 };
		var a2 = new Athlete { ID = id2 };
		athleteService.Get(id1).Returns(a1);
		athleteService.Get(id2).Returns(a2);
	adminService.Verify(id1).Returns(true);

		//act
		await controller.Merge(id1, id1, id2);

		//assert
		await adminService.Received().Delete(Arg.Any<Model.Admin>());
	}
}