using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Areas.Admin.Controllers;
using FLRC.Leaderboards.Web.ViewModels;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Admin;

public sealed class RacesControllerTests
{
	[Fact]
	public async Task CanGetRaceList()
	{
		//arrange
		var service = Substitute.For<IRaceService>();
		service.All().Returns([new Race(), new Race()]);

		var controller = new RacesController(service);

		//act
		var result = await controller.Index();

		//assert
		var model = result.Model as ViewModel<Race[]>;
		Assert.Equal(2, model!.Data.Length);
	}

	[Fact]
	public void CanGetAddForm()
	{
		//arrange
		var service = Substitute.For<IRaceService>();
		var controller = new RacesController(service);

		//act
		var result = controller.Add();

		//assert
		var model = result.Model as ViewModel<Race>;
		Assert.Equal("Add Race", model!.Title);
	}

	[Fact]
	public async Task CanAddRace()
	{
		//arrange
		var service = Substitute.For<IRaceService>();
		var controller = new RacesController(service);

		//act
		var race = new Race();
		var courses = new Dictionary<Guid, Course>();
		await controller.Add(race, courses);

		//assert
		await service.Received().Add(race);
		await service.Received().UpdateCourses(race, courses);
	}

	[Fact]
	public async Task CanGetEditForm()
	{
		//arrange
		var service = Substitute.For<IRaceService>();
		service.Get(Arg.Any<Guid>()).Returns(new Race { Name = "Test Race" });
		var controller = new RacesController(service);

		//act
		var result = await controller.Edit(Guid.NewGuid());

		//assert
		var model = result.Model as ViewModel<Race>;
		Assert.Equal("Edit Test Race", model!.Title);
	}

	[Fact]
	public async Task CanEditRace()
	{
		//arrange
		var id = Guid.NewGuid();
		var race = new Race { ID = id, Name = "Test Race" };
		var service = Substitute.For<IRaceService>();
		service.Get(id).Returns(race);
		var controller = new RacesController(service);

		//act
		var updated = new Race();
		var courses = new Dictionary<Guid, Course>();
		await controller.Edit(id, updated, courses);

		//assert
		await service.Received().Update(race, updated);
		await service.Received().UpdateCourses(race, courses);
	}
}