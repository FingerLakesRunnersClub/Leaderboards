using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Web.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests;

public sealed class AthleteControllerTests
{
	[Fact]
	public async Task PageContainsAllInfo()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var athlete = new Athlete { ID = 123, Category = Category.M };
		var course = new Course
		{
			Distance = new Distance("10 miles"),
			Results = new List<Result>
			{
				new Result
				{
					Athlete = athlete,
					Course = CourseData.Course,
					Duration = new Time(new TimeSpan(2, 4, 6)),
					CommunityStars =
					{
						[StarType.GroupRun] = true,
						[StarType.Story] = true
					}
				},
				new Result
				{
					Athlete = new Athlete { ID = 234, Category = Category.F },
					Course = CourseData.Course,
					Duration = new Time(new TimeSpan(1, 2, 3)),
					CommunityStars =
					{
						[StarType.GroupRun] = true,
						[StarType.Story] = true
					}
				}
			}
		};
		dataService.GetAthlete(123).Returns(athlete);
		dataService.GetAllResults().Returns(new List<Course> { course });
		var controller = new AthleteController(dataService, TestHelpers.Config);

		//act
		var response = await controller.Index(123);

		//assert
		var vm = (AthleteSummaryViewModel) response.Model;
		Assert.Equal((uint) 123, vm!.Summary.Athlete.ID);
		Assert.Equal(1, vm.Summary.Fastest[course].Rank.Value);
		Assert.Equal(1, vm.Summary.Average[course].Rank.Value);
		Assert.Equal(1, vm.Summary.Runs[course].Rank.Value);
		Assert.Equal(1, vm.Summary.CommunityStars[course].Rank.Value);
		Assert.Equal(1, vm.Summary.TeamResults.Rank.Value);
		Assert.Equal(100, vm.Summary.OverallPoints.Value.Value);
		Assert.Equal(10, vm.Summary.OverallMiles.Value.Value);
		Assert.Equal(2, vm.Summary.OverallCommunityStars.Value.Value);
	}

	[Fact]
	public async Task CanGetAllResultsForActivityLog()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var athlete = new Athlete { ID = 123 };
		var course = new Course
		{
			ID = 234,
			Distance = new Distance("10 miles"),
			Results = new List<Result>
			{
				new()
				{
					Athlete = athlete,
					Course = CourseData.Course,
					Duration = new Time(new TimeSpan(1, 2, 3))
				}
			}
		};
		dataService.GetAthlete(123).Returns(athlete);
		dataService.GetAllResults().Returns(new[] { course });
		var controller = new AthleteController(dataService, TestHelpers.Config);

		//act
		var response = await controller.Log(123);

		//assert
		var vm = (AthleteLogViewModel) response.Model;
		Assert.NotEmpty(vm!.Results);
	}

	[Fact]
	public async Task CanGetAllResultsForCourse()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var course = new Course
		{
			ID = 234,
			Distance = new Distance("10 miles"),
			Results = new List<Result>
			{
				new Result
				{
					Athlete = new Athlete { ID = 123 },
					Course = CourseData.Course,
					Duration = new Time(new TimeSpan(1, 2, 3))
				}
			}
		};
		dataService.GetResults(234, null).Returns(course);
		var controller = new AthleteController(dataService, TestHelpers.Config);

		//act
		var response = await controller.Course(123, 234, null);

		//assert
		var vm = (AthleteCourseResultsViewModel) response.Model;
		Assert.NotEmpty(vm!.RankedResults);
	}

	[Fact]
	public async Task ResultsAreRankedCorrectly()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var course = new Course
		{
			ID = 234,
			Distance = new Distance("10 miles"),
			Results = new List<Result>
			{
				new()
				{
					Athlete = new Athlete { ID = 123, Age = 35 },
					Course = CourseData.Course,
					Duration = new Time(new TimeSpan(0, 2, 3))
				},
				new()
				{
					Athlete = new Athlete { ID = 123, Age = 35 },
					Course = CourseData.Course,
					Duration = new Time(new TimeSpan(1, 2, 3))
				},
				new()
				{
					Athlete = new Athlete { ID = 123, Age = 35 },
					Course = CourseData.Course,
					Duration = new Time(new TimeSpan(2, 3, 4))
				},
				new()
				{
					Athlete = new Athlete { ID = 123, Age = 35 },
					Course = CourseData.Course,
					Duration = new Time(new TimeSpan(0, 2, 3))
				},
				new()
				{
					Athlete = new Athlete { ID = 123, Age = 35 },
					Course = CourseData.Course,
					Duration = new Time(new TimeSpan(1, 2, 3))
				}
			}
		};
		dataService.GetResults(234, null).Returns(course);
		var controller = new AthleteController(dataService, TestHelpers.Config);

		//act
		var response = await controller.Course(123, 234, null);

		//assert
		var vm = (AthleteCourseResultsViewModel) response.Model;
		Assert.Equal(0, vm!.RankedResults[0].Rank.Value);
		Assert.Equal(0, vm.RankedResults[1].Rank.Value);
		Assert.Equal(1, vm.RankedResults[2].Rank.Value);
		Assert.Equal(1, vm.RankedResults[3].Rank.Value);
		Assert.Equal(3, vm.RankedResults[4].Rank.Value);
	}

	[Fact]
	public async Task CanFindSimilarAthletes()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		dataService.GetAthlete(123).Returns(CourseData.Athlete1);
		dataService.GetAthlete(234).Returns(CourseData.Athlete2);
		dataService.GetAthlete(345).Returns(CourseData.Athlete3);
		dataService.GetAthlete(456).Returns(CourseData.Athlete4);
		dataService.GetAllResults().Returns(new[] { new Course { Results = CourseData.SimilarResults, Distance = new Distance("400m") } });

		var controller = new AthleteController(dataService, TestHelpers.Config);

		//act
		var result = await controller.Similar(123);

		//assert
		var vm = (SimilarAthletesViewModel) result.Model;
		var matches = vm!.Matches.ToArray();
		Assert.Equal(CourseData.Athlete1, vm.Athlete);
		Assert.Equal(CourseData.Athlete4, matches[0].Athlete);
		Assert.Equal("96%", matches[0].Similarity.Display);
		Assert.Equal(CourseData.Athlete2, matches[1].Athlete);
		Assert.Equal("95%", matches[1].Similarity.Display);
	}
}