using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Athletes;

public sealed class AthleteSummaryTests
{
	[Fact]
	public void CanGetTotalNumberOfResults()
	{
		//arrange
		var course1 = new Course { Results = CourseData.Results, Distance = new Distance("10K") };
		var course2 = new Course { Results = CourseData.Results, Distance = new Distance("10K") };
		var course3 = new Course { Results = CourseData.Results, Distance = new Distance("10K") };
		var results = new[] { course1, course2, course3 };

		//act
		var summary = new AthleteSummary(CourseData.Athlete1, results, TestHelpers.Config);

		//assert
		Assert.Equal(6, summary.TotalResults);
	}

	[Fact]
	public void SimilarAthletesSelectsOnlyCloseTimes()
	{
		//arrange
		var results = new[] { new Course { Results = CourseData.SimilarResults, Distance = new Distance("400m") } };
		var summary = new AthleteSummary(CourseData.Athlete1, results, TestHelpers.Config);

		//act
		var similar = summary.SimilarAthletes();

		//assert
		Assert.Equal(2, similar.Length);
		Assert.Equal(CourseData.Athlete4, similar[0].Athlete);
		Assert.Equal(CourseData.Athlete2, similar[1].Athlete);
	}

	[Fact]
	public void SimilarAthletesFiltersOutliers()
	{
		//arrange
		var course = new Course { Distance = new Distance("400m") };
		var results = new[]
		{
			new Result { Course = course, Athlete = CourseData.Athlete1, Duration = new Time(TimeSpan.FromSeconds(100)) },
			new Result { Course = course, Athlete = CourseData.Athlete1, Duration = new Time(TimeSpan.FromSeconds(101)) },
			new Result { Course = course, Athlete = CourseData.Athlete1, Duration = new Time(TimeSpan.FromSeconds(102)) },
			new Result { Course = course, Athlete = CourseData.Athlete2, Duration = new Time(TimeSpan.FromSeconds(103)) },
			new Result { Course = course, Athlete = CourseData.Athlete2, Duration = new Time(TimeSpan.FromSeconds(104)) },
			new Result { Course = course, Athlete = CourseData.Athlete2, Duration = new Time(TimeSpan.FromSeconds(105)) },
			new Result { Course = course, Athlete = CourseData.Athlete3, Duration = new Time(TimeSpan.FromSeconds(95)) },
			new Result { Course = course, Athlete = CourseData.Athlete3, Duration = new Time(TimeSpan.FromSeconds(200)) },
			new Result { Course = course, Athlete = CourseData.Athlete3, Duration = new Time(TimeSpan.FromSeconds(300)) }
		};
		course.Results = results;
		var courses = new[] { course };
		var summary = new AthleteSummary(CourseData.Athlete1, courses, TestHelpers.Config);

		//act
		var similar = summary.SimilarAthletes();

		//assert
		Assert.Equal(CourseData.Athlete2, similar.Single().Athlete);
	}
}