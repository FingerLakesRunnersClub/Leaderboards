using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Areas.Admin;
using FLRC.Leaderboards.Web.Areas.Admin.Controllers;
using FLRC.Leaderboards.Web.Areas.Admin.ViewModels;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Admin;

public sealed class AthleteControllerTests
{
	[Fact]
	public async Task CanViewListOfAthletes()
	{
		//arrange
		var service = Substitute.For<IAthleteService>();
		var controller = new AthletesController(service);

		service.All().Returns([new Athlete(), new Athlete()]);

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
		var service = Substitute.For<IAthleteService>();
		var controller = new AthletesController(service);

		var id = Guid.NewGuid();
		service.Get(id).Returns(new Athlete());

		//act
		var results = await controller.Edit(id);

		//assert
		Assert.Equal("Form", results.ViewName);
	}

	[Fact]
	public async Task CanEditAthlete()
	{
		//arrange
		var service = Substitute.For<IAthleteService>();
		var controller = new AthletesController(service);

		var id = Guid.NewGuid();
		service.Get(id).Returns(new Athlete());

		//act
		await controller.Edit(id, new Athlete());

		//assert
		await service.Received().Update(Arg.Any<Athlete>(), Arg.Any<Athlete>());
	}

	[Fact]
	public async Task ToggleAdminAddsAdminWhenNotSet()
	{
		//arrange
		var service = Substitute.For<IAthleteService>();
		var controller = new AthletesController(service);

		var id = Guid.NewGuid();
		service.Get(id).Returns(new Athlete());

		//act
		await controller.ToggleAdmin(id);

		//assert
		await service.Received().AddAdmin(Arg.Any<Athlete>());
	}

	[Fact]
	public async Task ToggleAdminRemovesAdminWhenAlreadySet()
	{
		//arrange
		var service = Substitute.For<IAthleteService>();
		var controller = new AthletesController(service);

		var id = Guid.NewGuid();
		service.Get(id).Returns(new Athlete { Admins = [new Model.Admin()] });

		//act
		await controller.ToggleAdmin(id);

		//assert
		await service.Received().RemoveAdmin(Arg.Any<Athlete>());
	}

	[Fact]
	public async Task CanViewMergeForm()
	{
		//arrange
		var service = Substitute.For<IAthleteService>();
		var controller = new AthletesController(service);

		var id = Guid.NewGuid();
		service.Get(id).Returns(new Athlete());
		service.All().Returns([new Athlete(), new Athlete()]);

		//act
		var result = await controller.Merge(id);

		//assert
		Assert.IsType<ViewModel<MergeAthletesForm>>(result.Model);
	}

	[Fact]
	public async Task MergeRequiresCurrentAthleteToBeSet()
	{
		//arrange
		var service = Substitute.For<IAthleteService>();
		var controller = new AthletesController(service);

		//act
		var task = async () => await controller.Merge(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

		//assert
		await Assert.ThrowsAsync<ArgumentException>(task);
	}

	[Fact]
	public async Task MergeRequiresDifferentAthletesToBeSet()
	{
		//arrange
		var service = Substitute.For<IAthleteService>();
		var controller = new AthletesController(service);

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
		var service = Substitute.For<IAthleteService>();
		var controller = new AthletesController(service);

		var id1 = Guid.NewGuid();
		var id2 = Guid.NewGuid();
		var a1 = new Athlete { ID = id1 };
		var a2 = new Athlete { ID = id2 };
		service.Get(id1).Returns(a1);
		service.Get(id2).Returns(a2);

		//act
		await controller.Merge(id1, id1, id2);

		//assert
		await service.Received().MigrateResults(a1, a2);
		await service.Received().MigrateRegistrations(a1, a2);
		await service.Received().MigrateLinkedAccounts(a1, a2);
		await service.Received().RemoveAdmin(a1);
		await service.Received().Delete(a1);
	}
}