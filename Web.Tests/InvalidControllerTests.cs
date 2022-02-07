using FLRC.Leaderboards.Core;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Reports;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Web.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web;

public class InvalidControllerTests
{
	[Fact]
	public async Task InvalidResultsAreEmptyWhenAllResultsAreGood()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var course = new Course
		{
			Distance = new Distance("10K"),
			Results = CourseData.Results
		};
		dataService.GetAllResults().Returns(new[] { course });
		var controller = new InvalidController(dataService, TestHelpers.Config);

		//act
		var result = await controller.Index();

		//assert
		var model = (InvalidViewModel)result.Model;
		Assert.Empty(model.Results);
	}

	[Fact]
	public async Task InvalidResultsExistWhenTooFast()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var badResult = new Result
		{
			Athlete = CourseData.Athlete1,
			StartTime = new Date(new DateTime(2021, 7, 1)),
			Duration = new Time(TimeSpan.FromMinutes(5))
		};
		var results = CourseData.Results.ToList();
		results.Add(badResult);
		var course = new Course
		{
			Distance = new Distance("10K"),
			Results = results
		};
		dataService.GetAllResults().Returns(new[] { course });
		var controller = new InvalidController(dataService, TestHelpers.Config);

		//act
		var result = await controller.Index();

		//assert
		var model = (InvalidViewModel)result.Model;
		Assert.Equal(1, model.Results.Count);
	}
}