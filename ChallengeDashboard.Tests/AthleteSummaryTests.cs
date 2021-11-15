using System;
using System.Linq;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests;

public class AthleteSummaryTests
{
	[Fact]
	public void CanGetTotalNumberOfResults()
	{
		//arrange
		var course1 = new Course { Results = CourseData.Results };
		var course2 = new Course { Results = CourseData.Results };
		var course3 = new Course { Results = CourseData.Results };
		var results = new[] { course1, course2, course3 };

		//act
		var summary = new AthleteSummary(CourseData.Athlete1, results);

		//assert
		Assert.Equal(6, summary.TotalResults);
	}

	[Fact]
	public void SimilarAthletesSelectsOnlyCloseTimes()
	{
		//arrange
		var results = new[] { new Course { Results = CourseData.SimilarResults } };
		var summary = new AthleteSummary(CourseData.Athlete1, results);

		//act
		var similar = summary.SimilarAthletes.ToList();

		//assert
		Assert.Equal(2, similar.Count);
		Assert.Equal(CourseData.Athlete4, similar[0].Athlete);
		Assert.Equal(CourseData.Athlete2, similar[1].Athlete);
	}

	[Fact]
	public void SimilarAthletesFiltersOutliers()
	{
		//arrange
		var course = new Course
		{
			Results = new[]
			{
					new Result { Athlete = CourseData.Athlete1, Duration = new Time(TimeSpan.FromSeconds(100))},
					new Result { Athlete = CourseData.Athlete1, Duration = new Time(TimeSpan.FromSeconds(101))},
					new Result { Athlete = CourseData.Athlete1, Duration = new Time(TimeSpan.FromSeconds(102))},
					new Result { Athlete = CourseData.Athlete2, Duration = new Time(TimeSpan.FromSeconds(103))},
					new Result { Athlete = CourseData.Athlete2, Duration = new Time(TimeSpan.FromSeconds(104))},
					new Result { Athlete = CourseData.Athlete2, Duration = new Time(TimeSpan.FromSeconds(105))},
					new Result { Athlete = CourseData.Athlete3, Duration = new Time(TimeSpan.FromSeconds(95))},
					new Result { Athlete = CourseData.Athlete3, Duration = new Time(TimeSpan.FromSeconds(200))},
					new Result { Athlete = CourseData.Athlete3, Duration = new Time(TimeSpan.FromSeconds(300))}
				}
		};
		var results = new[] { course };
		var summary = new AthleteSummary(CourseData.Athlete1, results);

		//act
		var similar = summary.SimilarAthletes.ToList();

		//assert
		Assert.Equal(CourseData.Athlete2, similar.Single().Athlete);
	}
}
