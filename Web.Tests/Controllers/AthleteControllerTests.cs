using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Controllers;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Controllers;

public sealed class AthleteControllerTests
{
	[Fact]
	public async Task CanGetAllResultsForActivityLog()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var authService = Substitute.For<IAuthService>();
		var adminService = Substitute.For<IAdminService>();
		var athleteService = Substitute.For<IAthleteService>();
		var courseService = Substitute.For<ICourseService>();
		var summaryCalculator = Substitute.For<IAthleteSummaryCalculator>();

		var controller = new AthleteController(iterationManager, authService, adminService, athleteService, courseService, summaryCalculator);

		var athlete = ResultsData.Athlete1 with
		{
			Results =
			[
				new Result
				{
					Athlete = ResultsData.Athlete1,
					Course = ResultsData.Course,
					Duration = new TimeSpan(1, 2, 3)
				}
			]
		};
		athleteService.Get(athlete.ID).Returns(athlete);

		//act
		var response = await controller.Log(athlete.ID);

		//assert
		var vm = response.Model as ViewModel<AthleteLog>;
		Assert.NotEmpty(vm!.Data.Results);
	}

	[Fact]
	public async Task CanGetAllResultsForCourse()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var authService = Substitute.For<IAuthService>();
		var adminService = Substitute.For<IAdminService>();
		var athleteService = Substitute.For<IAthleteService>();
		var courseService = Substitute.For<ICourseService>();
		var summaryCalculator = Substitute.For<IAthleteSummaryCalculator>();

		var controller = new AthleteController(iterationManager, authService, adminService, athleteService, courseService, summaryCalculator);

		var athlete = new Athlete { ID = Guid.NewGuid() };
		athleteService.Get(athlete.ID).Returns(athlete);

		var course = new Course
		{
			ID = Guid.NewGuid(),
			Race = new Race { Name = "Test" },
			Distance = 10,
			Units = "mi",
			Results =
			[
				new Result
				{
					Athlete = athlete,
					Course = ResultsData.Course,
					Duration = new TimeSpan(1, 2, 3)
				}
			]
		};
		courseService.Get(course.ID).Returns(course);

		//act
		var response = await controller.Course(athlete.ID, course.ID);

		//assert
		var vm = response.Model as ViewModel<AthleteCourseResults<Time>>;
		Assert.NotEmpty(vm!.Data.RankedResults);
	}

	[Fact]
	public async Task ResultsAreRankedCorrectly()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var authService = Substitute.For<IAuthService>();
		var adminService = Substitute.For<IAdminService>();
		var athleteService = Substitute.For<IAthleteService>();
		var courseService = Substitute.For<ICourseService>();
		var summaryCalculator = Substitute.For<IAthleteSummaryCalculator>();

		var controller = new AthleteController(iterationManager, authService, adminService, athleteService, courseService, summaryCalculator);

		var athlete = new Athlete { ID = Guid.NewGuid(), DateOfBirth = new DateOnly(2000, 1, 1)};
		athleteService.Get(athlete.ID).Returns(athlete);

		var course = new Course
		{
			ID = Guid.NewGuid(),
			Race = new Race { Name = "Test" },
			Distance = 10,
			Units = "mi",
			Results =
			[
				new Result
				{
					Athlete = athlete,
					Course = ResultsData.Course,
					StartTime = new DateTime(2026, 1, 1),
					Duration = new TimeSpan(0, 2, 3)
				},
				new Result
				{
					Athlete = athlete,
					Course = ResultsData.Course,
					StartTime = new DateTime(2026, 1, 2),
					Duration = new TimeSpan(1, 2, 3)
				},
				new Result
				{
					Athlete = athlete,
					Course = ResultsData.Course,
					StartTime = new DateTime(2026, 1, 3),
					Duration = new TimeSpan(2, 3, 4)
				},
				new Result
				{
					Athlete = athlete,
					Course = ResultsData.Course,
					StartTime = new DateTime(2026, 1, 4),
					Duration = new TimeSpan(0, 2, 3)
				},
				new Result
				{
					Athlete = athlete,
					Course = ResultsData.Course,
					StartTime = new DateTime(2026, 1, 5),
					Duration = new TimeSpan(1, 2, 3)
				}
			]
		};
		courseService.Get(course.ID).Returns(course);

		//act
		var response = await controller.Course(athlete.ID, course.ID);

		//assert
		var vm = response.Model as ViewModel<AthleteCourseResults<Time>>;
		Assert.Equal(0, vm!.Data.RankedResults[0].Rank.Value);
		Assert.Equal(0, vm.Data.RankedResults[1].Rank.Value);
		Assert.Equal(1, vm.Data.RankedResults[2].Rank.Value);
		Assert.Equal(1, vm.Data.RankedResults[3].Rank.Value);
		Assert.Equal(3, vm.Data.RankedResults[4].Rank.Value);
	}
}