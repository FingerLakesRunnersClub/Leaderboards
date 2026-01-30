using FLRC.Leaderboards.Data.Models;
using FLRC.Leaderboards.Web.Areas.Admin;
using FLRC.Leaderboards.Web.Areas.Admin.Controllers;
using FLRC.Leaderboards.Web.Areas.Admin.Services;
using FLRC.Leaderboards.Web.Areas.Admin.ViewModels;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Admin;

public class IterationsControllerTests
{
	[Fact]
	public async Task CanGetIterationsList()
	{
		//arrange
		var seriesService = Substitute.For<ISeriesService>();
		var iterationService = Substitute.For<IIterationService>();
		var raceService = Substitute.For<IRaceService>();
		var controller = new IterationsController(seriesService, iterationService, raceService);

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
		var controller = new IterationsController(seriesService, iterationService, raceService);

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
		var controller = new IterationsController(seriesService, iterationService, raceService);

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
		var controller = new IterationsController(seriesService, iterationService, raceService);

		//act
		var seriesID = Guid.NewGuid();
		var iteration = new Iteration();
		await controller.Add(seriesID, iteration, []);

		//assert
		await iterationService.Received().AddIteration(seriesID, iteration, []);
	}

	[Fact]
	public async Task CanGetEditIterationForm()
	{
		//arrange
		var seriesService = Substitute.For<ISeriesService>();
		var iterationService = Substitute.For<IIterationService>();
		var iteration = new Iteration { Name = "Iteration", Series = new Series { Name = "Test" } };
		var raceService = Substitute.For<IRaceService>();
		var controller = new IterationsController(seriesService, iterationService, raceService);

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
		var controller = new IterationsController(seriesService, iterationService, raceService);

		var id = Guid.NewGuid();
		var iteration = new Iteration { ID = id, Name = "Iteration", Series = new Series { Name = "Test" } };
		iterationService.GetIteration(Arg.Any<Guid>()).Returns(iteration);

		//act
		var updated = new Iteration();
		await controller.Edit(id, updated, []);

		//assert
		await iterationService.Received().UpdateIteration(iteration, updated, []);
	}
}