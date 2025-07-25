using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Reports;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Web.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests;

public sealed class InvalidControllerTests
{
	[Fact]
	public async Task InvalidResultsAreEmptyWhenAllResultsAreGood()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var course = new Course
		{
			Race = new Race(),
			Distance = new Distance("10K"),
			Results = CourseData.Results
		};
		dataService.GetAllResults().Returns([course]);
		var controller = new InvalidController(dataService, TestHelpers.Config);

		//act
		var result = await controller.Index();

		//assert
		var model = (InvalidViewModel)result.Model;
		Assert.Empty(model!.Results);
	}

	[Fact]
	public async Task InvalidResultsExistWhenTooFast()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var badResult = new Result
		{
			Course = CourseData.Course,
			Athlete = CourseData.Athlete1,
			StartTime = new Date(new DateTime(2021, 7, 1)),
			Duration = new Time(TimeSpan.FromMinutes(5))
		};

		var results = CourseData.Results.ToList();
		results.Add(badResult);
		var course = new Course
		{
			Race = new Race(),
			Distance = new Distance("10K"),
			Results = results.ToArray()
		};
		dataService.GetAllResults().Returns([course]);
		var controller = new InvalidController(dataService, TestHelpers.Config);

		//act
		var result = await controller.Index();

		//assert
		var model = (InvalidViewModel)result.Model;
		Assert.Single(model!.Results);
	}
}