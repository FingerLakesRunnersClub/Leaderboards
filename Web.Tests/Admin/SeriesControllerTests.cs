using FLRC.Leaderboards.Data.Models;
using FLRC.Leaderboards.Web.Areas.Admin;
using FLRC.Leaderboards.Web.Areas.Admin.Controllers;
using FLRC.Leaderboards.Web.Areas.Admin.Services;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Admin;

public sealed class SeriesControllerTests
{
	[Fact]
	public async Task CanGetSeriesList()
	{
		//arrange
		var service = Substitute.For<ISeriesService>();
		service.GetAllSeries().Returns([new Series(), new Series()]);
		var controller = new SeriesController(service);

		//act
		var result = await controller.Index();

		//assert
		var model = result.Model as ViewModel<Series[]>;
		Assert.Equal(2, model!.Data.Length);
	}

	[Fact]
	public void CanGetAddSeriesForm()
	{
		//arrange
		var service = Substitute.For<ISeriesService>();
		var controller = new SeriesController(service);

		//act
		var result = controller.Add();

		//assert
		var model = result.Model as Core.ViewModel;
		Assert.Equal("Add Series", model!.Title);
	}

	[Fact]
	public async Task CanAddSeries()
	{
		//arrange
		var service = Substitute.For<ISeriesService>();
		var controller = new SeriesController(service);

		//act
		var series = new Series();
		var features = new Dictionary<string, bool>();
		var settings = new Dictionary<string, string>();
		await controller.Add(series, features, settings);

		//assert
		await service.Received().AddSeries(series, features, settings);
	}

	[Fact]
	public async Task CanGetEditSeriesForm()
	{
		//arrange
		var service = Substitute.For<ISeriesService>();
		service.GetSeries(Arg.Any<Guid>()).Returns(new Series());
		var controller = new SeriesController(service);

		//act
		var result = await controller.Edit(Guid.NewGuid());

		//assert
		var model = result!.Model as Core.ViewModel;
		Assert.Equal("Edit Series", model!.Title);
	}

	[Fact]
	public async Task CanEditSeries()
	{
		//arrange
		var id = Guid.NewGuid();
		var series = new Series();
		var service = Substitute.For<ISeriesService>();
		service.GetSeries(id).Returns(series);
		var controller = new SeriesController(service);

		//act
		var updated = new Series();
		var features = new Dictionary<string, bool>();
		var settings = new Dictionary<string, string>();
		await controller.Edit(id, updated, features, settings);

		//assert
		await service.Received().UpdateSeries(series, updated, features, settings);
	}
}