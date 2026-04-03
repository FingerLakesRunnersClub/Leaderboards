using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Controllers;
using FLRC.Leaderboards.Web.ViewModels;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Controllers;

public sealed class LogControllerTests
{
	[Fact]
	public async Task RecentShowsResultsInReverseChronologicalOrder()
	{
		//arrange
		var courseService = Substitute.For<ICourseService>();
		var resultService = Substitute.For<IResultService>();

		var controller = new LogController(courseService, resultService, new DateTime(2020, 2, 8));

		var course = new Course { Results = ResultsData.Results };
		courseService.Get(Arg.Any<Guid>()).Returns(course);
		resultService.All().Returns(course.Results.ToArray());

		//act
		var response = await controller.Index();

		//assert
		var vm = response.Model as ViewModel<ActivityLog>;
		var results = vm!.Data.Results.SelectMany(g => g).ToArray();
		Assert.Equal(7, results.Length);
		Assert.Equal(new DateTime(2020, 2, 8), results[0].StartTime);
		Assert.Equal(new DateTime(2020, 2, 7), results[1].StartTime);
		Assert.Equal(new DateTime(2020, 2, 6), results[2].StartTime);
		Assert.Equal(new DateTime(2020, 2, 5), results[3].StartTime);
		Assert.Equal(new DateTime(2020, 2, 4), results[4].StartTime);
		Assert.Equal(new DateTime(2020, 2, 3), results[5].StartTime);
		Assert.Equal(new DateTime(2020, 2, 2), results[6].StartTime);
	}

	[Fact]
	public async Task ArchiveLogShowsResultsInReverseChronologicalOrder()
	{
		//arrange
		var courseService = Substitute.For<ICourseService>();
		var resultService = Substitute.For<IResultService>();

		var controller = new LogController(courseService, resultService, new DateTime(2020, 2, 8));

		var course = new Course { Results = ResultsData.Results };
		courseService.Get(Arg.Any<Guid>()).Returns(course);
		resultService.All().Returns(course.Results.ToArray());

		//act
		var response = await controller.All();

		//assert
		var vm = response.Model as ViewModel<ActivityLog>;
		var results = vm!.Data.Results.SelectMany(g => g).ToArray();
		Assert.Equal(8, results.Length);
		Assert.Equal(new DateTime(2020, 2, 8), results[0].StartTime);
		Assert.Equal(new DateTime(2020, 2, 7), results[1].StartTime);
		Assert.Equal(new DateTime(2020, 2, 6), results[2].StartTime);
		Assert.Equal(new DateTime(2020, 2, 5), results[3].StartTime);
		Assert.Equal(new DateTime(2020, 2, 4), results[4].StartTime);
		Assert.Equal(new DateTime(2020, 2, 3), results[5].StartTime);
		Assert.Equal(new DateTime(2020, 2, 2), results[6].StartTime);
		Assert.Equal(new DateTime(2020, 2, 1), results[7].StartTime);
	}

	[Fact]
	public async Task SameMonthsInDifferentYearsAreSeparated()
	{
		//arrange
		var courseService = Substitute.For<ICourseService>();
		var resultService = Substitute.For<IResultService>();

		var controller = new LogController(courseService, resultService, new DateTime(2020, 2, 8));

		var results = ResultsData.Results.ToList();
		results.Add(new Result { StartTime = new DateTime(2021, 2, 1) });
		var course = new Course { Results = results.ToArray() };
		courseService.Get(Arg.Any<Guid>()).Returns(course);
		resultService.All().Returns(course.Results.ToArray());

		//act
		var response = await controller.All();

		//assert
		var vm = response.Model as ViewModel<ActivityLog>;
		var groups = vm!.Data.Results.Select(g => g.Key).ToArray();
		Assert.Equal(2, groups.Length);
		Assert.Equal("February 2021", groups[0]);
		Assert.Equal("February 2020", groups[1]);
	}

	[Fact]
	public async Task CanGetActivityLogForCourse()
	{
		//arrange
		var courseService = Substitute.For<ICourseService>();
		var resultService = Substitute.For<IResultService>();

		var controller = new LogController(courseService, resultService, new DateTime(2020, 2, 8));

		var course = new Course { ID = Guid.NewGuid(), Race = new Race { Name = "Test" }, Results = ResultsData.Results };
		courseService.Get(Arg.Any<Guid>()).Returns(course);
		resultService.Find(course.ID).Returns(course.Results.ToArray());

		//act
		var response = await controller.Index(course.ID);

		//assert
		var vm = response.Model as ViewModel<ActivityLog>;
		var results = vm!.Data.Results.SelectMany(g => g).ToArray();
		Assert.Equal(7, results.Length);
		Assert.Equal(new DateTime(2020, 2, 8), results[0].StartTime);
		Assert.Equal(new DateTime(2020, 2, 7), results[1].StartTime);
		Assert.Equal(new DateTime(2020, 2, 6), results[2].StartTime);
		Assert.Equal(new DateTime(2020, 2, 5), results[3].StartTime);
		Assert.Equal(new DateTime(2020, 2, 4), results[4].StartTime);
		Assert.Equal(new DateTime(2020, 2, 3), results[5].StartTime);
		Assert.Equal(new DateTime(2020, 2, 2), results[6].StartTime);
	}
}