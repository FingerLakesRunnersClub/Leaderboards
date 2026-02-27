using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Areas.Admin;
using FLRC.Leaderboards.Web.Areas.Admin.Controllers;
using FLRC.Leaderboards.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Admin;

public sealed class IterationsControllerTests
{
	[Fact]
	public async Task CanGetIterationsList()
	{
		//arrange
		var seriesService = Substitute.For<ISeriesService>();
		var iterationService = Substitute.For<IIterationService>();
		var raceService = Substitute.For<IRaceService>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new IterationsController(seriesService, iterationService, raceService, registrationManager);

		iterationService.GetAllIterations().Returns([
			new Iteration { Series = new Series { ID = Guid.NewGuid() } },
			new Iteration { Series = new Series { ID = Guid.NewGuid() } }
		]);

		//act
		var result = await controller.Index();

		//assert
		var model = result.Model as ViewModel<IGrouping<Series, Iteration>[]>;
		Assert.Equal(2, model!.Data.Length);
	}

	[Fact]
	public async Task IterationsListIsGroupedBySeries()
	{
		//arrange
		var seriesService = Substitute.For<ISeriesService>();
		var iterationService = Substitute.For<IIterationService>();
		var raceService = Substitute.For<IRaceService>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new IterationsController(seriesService, iterationService, raceService, registrationManager);

		var series = new Series { ID = Guid.NewGuid() };
		iterationService.GetAllIterations().Returns([
			new Iteration { Series = series },
			new Iteration { Series = series }
		]);

		//act
		var result = await controller.Index();

		//assert
		var model = result.Model as ViewModel<IGrouping<Series, Iteration>[]>;
		Assert.Single(model!.Data);
	}

	[Fact]
	public async Task CanGetAddIterationForm()
	{
		//arrange
		var seriesService = Substitute.For<ISeriesService>();
		var iterationService = Substitute.For<IIterationService>();
		var raceService = Substitute.For<IRaceService>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new IterationsController(seriesService, iterationService, raceService, registrationManager);

		seriesService.GetSeries(Arg.Any<Guid>()).Returns(new Series { Name = "Test" });

		//act
		var result = await controller.Add(Guid.NewGuid());

		//assert
		var model = result.Model as ViewModel<IterationForm>;
		Assert.Equal("Add Iteration to Test", model!.Title);
	}

	[Fact]
	public async Task CanAddIteration()
	{
		//arrange
		var seriesService = Substitute.For<ISeriesService>();
		var iterationService = Substitute.For<IIterationService>();
		var raceService = Substitute.For<IRaceService>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new IterationsController(seriesService, iterationService, raceService, registrationManager);

		//act
		var seriesID = Guid.NewGuid();
		var iteration = new Iteration();
		await controller.Add(seriesID, iteration, []);

		//assert
		await iterationService.Received().AddIteration(seriesID, iteration);
		await iterationService.Received().UpdateRaces(iteration, Arg.Any<Race[]>());
	}

	[Fact]
	public async Task CanGetEditIterationForm()
	{
		//arrange
		var seriesService = Substitute.For<ISeriesService>();
		var iterationService = Substitute.For<IIterationService>();
		var iteration = new Iteration { Name = "Iteration", Series = new Series { Name = "Test" } };
		var raceService = Substitute.For<IRaceService>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new IterationsController(seriesService, iterationService, raceService, registrationManager);

		iterationService.GetIteration(Arg.Any<Guid>()).Returns(iteration);

		//act
		var result = await controller.Edit(Guid.NewGuid());

		//assert
		var model = result.Model as Core.ViewModel;
		Assert.Equal("Edit Test Iteration", model!.Title);
	}

	[Fact]
	public async Task CanEditIteration()
	{
		//arrange
		var seriesService = Substitute.For<ISeriesService>();
		var iterationService = Substitute.For<IIterationService>();
		var raceService = Substitute.For<IRaceService>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new IterationsController(seriesService, iterationService, raceService, registrationManager);

		var id = Guid.NewGuid();
		var iteration = new Iteration { ID = id, Name = "Iteration", Series = new Series { Name = "Test" } };
		iterationService.GetIteration(Arg.Any<Guid>()).Returns(iteration);

		//act
		var updated = new Iteration();
		await controller.Edit(id, updated, []);

		//assert
		await iterationService.Received().UpdateIteration(iteration, updated);
		await iterationService.Received().UpdateRaces(iteration, Arg.Any<Race[]>());
	}

	[Fact]
	public async Task CanViewRegistrations()
	{
		//arrange
		var seriesService = Substitute.For<ISeriesService>();
		var iterationService = Substitute.For<IIterationService>();
		var raceService = Substitute.For<IRaceService>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new IterationsController(seriesService, iterationService, raceService, registrationManager);

		var id = Guid.NewGuid();
		var iteration = new Iteration
		{
			ID = id,
			Name = "Iteration",
			Series = new Series { Name = "Test" },
			Athletes =
			[
				new Athlete(),
				new Athlete()
			]
		};
		iterationService.GetIteration(Arg.Any<Guid>()).Returns(iteration);

		//act
		var result = await controller.Registration(id);

		//assert
		var model = result.Model as ViewModel<Athlete[]>;
		Assert.Equal(2, model!.Data.Length);
	}

	[Fact]
	public async Task CanUpdateIterationRegistrations()
	{
		//arrange
		var seriesService = Substitute.For<ISeriesService>();
		var iterationService = Substitute.For<IIterationService>();
		var raceService = Substitute.For<IRaceService>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new IterationsController(seriesService, iterationService, raceService, registrationManager);

		var id = Guid.NewGuid();
		var iteration = new Iteration { ID = id, Name = "Iteration", Series = new Series { Name = "Test" } };
		iterationService.GetIteration(Arg.Any<Guid>()).Returns(iteration);

		//act
		await controller.Registration(id, new FormCollection([]));

		//assert
		await registrationManager.Received().Update(iteration);
	}
}