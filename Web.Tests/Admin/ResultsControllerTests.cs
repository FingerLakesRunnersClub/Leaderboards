using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Areas.Admin;
using FLRC.Leaderboards.Web.Areas.Admin.Controllers;
using FLRC.Leaderboards.Web.Areas.Admin.ViewModels;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Admin;

public sealed class ResultsControllerTests
{
	[Fact]
	public async Task CanViewResultsForCourse()
	{
		//arrange
		var importManager = Substitute.For<IImportManager>();
		var courseService = Substitute.For<ICourseService>();
		var raceService = Substitute.For<IRaceService>();
		var resultService = Substitute.For<IResultService>();
		var controller = new ResultsController(importManager, courseService, raceService, resultService);

		var courseID = Guid.NewGuid();
		resultService.Find(courseID).Returns([new Result(), new Result()]);

		//act
		var result = await controller.Index(courseID);

		//assert
		var model = result.Model as ViewModel<Result[]>;
		Assert.Equal(2, model!.Data.Length);
	}

	[Fact]
	public async Task CanShowImportForm()
	{
		//arrange
		var importManager = Substitute.For<IImportManager>();
		var courseService = Substitute.For<ICourseService>();
		var raceService = Substitute.For<IRaceService>();
		var resultService = Substitute.For<IResultService>();
		var controller = new ResultsController(importManager, courseService, raceService, resultService);

		//act
		var result = await controller.Import();

		//assert
		Assert.IsType<ViewModel<ResultImportForm>>(result.Model);
	}

	[Fact]
	public async Task CanImportResults()
	{
		//arrange
		var importManager = Substitute.For<IImportManager>();
		var courseService = Substitute.For<ICourseService>();
		var raceService = Substitute.For<IRaceService>();
		var resultService = Substitute.For<IResultService>();
		var controller = new ResultsController(importManager, courseService, raceService, resultService);

		//act
		var courseID = Guid.NewGuid();
		await controller.Import(courseID, "test", 123);

		//assert
		await importManager.Received().ImportResults(Arg.Any<Course>(), "test", 123);
	}
}