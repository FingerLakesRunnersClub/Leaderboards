using FLRC.Leaderboards.Data.Models;
using FLRC.Leaderboards.Web.Areas.Admin;
using FLRC.Leaderboards.Web.Areas.Admin.Controllers;
using FLRC.Leaderboards.Web.Areas.Admin.Services;
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
		iterationService.GetAllIterations().Returns([
			new Iteration { Series = new Series { ID = Guid.NewGuid() } },
			new Iteration { Series = new Series { ID = Guid.NewGuid() } }
		]);
		var controller = new IterationsController(seriesService, iterationService);

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
		var series = new Series { ID = Guid.NewGuid() };
		iterationService.GetAllIterations().Returns([
			new Iteration { Series = series },
			new Iteration { Series = series }
		]);
		var controller = new IterationsController(seriesService, iterationService);

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
		seriesService.GetSeries(Arg.Any<Guid>()).Returns(new Series { Name = "Test" });
		var iterationService = Substitute.For<IIterationService>();
		var controller = new IterationsController(seriesService, iterationService);

		//act
		var result = await controller.Add(Guid.NewGuid());

		//assert
		var model = result.Model as ViewModel<Iteration>;
		Assert.Equal("Add Iteration to Test", model!.Title);
	}

	[Fact]
	public async Task CanAddIteration()
	{
		//arrange
		var seriesService = Substitute.For<ISeriesService>();
		var iterationService = Substitute.For<IIterationService>();
		var controller = new IterationsController(seriesService, iterationService);

		//act
		var seriesID = Guid.NewGuid();
		var iteration = new Iteration();
		await controller.Add(seriesID, iteration);

		//assert
		await iterationService.Received().AddIteration(seriesID, iteration);
	}

	[Fact]
	public async Task CanGetEditIterationForm()
	{
		//arrange
		var seriesService = Substitute.For<ISeriesService>();
		var iterationService = Substitute.For<IIterationService>();
		var iteration = new Iteration { Name = "Iteration", Series = new Series { Name = "Test" } };
		iterationService.GetIteration(Arg.Any<Guid>()).Returns(iteration);
		var controller = new IterationsController(seriesService, iterationService);

		//act
		var result = await controller.Edit(Guid.NewGuid());

		//assert
		var model = result.Model as ViewModel<Iteration>;
		Assert.Equal("Edit Test Iteration", model!.Title);
	}

	[Fact]
	public async Task CanEditIteration()
	{
		//arrange
		var id = Guid.NewGuid();
		var iteration = new Iteration { ID = id, Name = "Iteration", Series = new Series { Name = "Test" } };
		var seriesService = Substitute.For<ISeriesService>();
		var iterationService = Substitute.For<IIterationService>();
		iterationService.GetIteration(Arg.Any<Guid>()).Returns(iteration);
		var controller = new IterationsController(seriesService, iterationService);

		//act
		var updated = new Iteration();
		await controller.Edit(id, updated);

		//assert
		await iterationService.Received().UpdateIteration(iteration, updated);
	}
}