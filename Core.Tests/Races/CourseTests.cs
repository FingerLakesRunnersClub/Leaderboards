using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Races;

public sealed class CourseTests
{
	[Fact]
	public void CanGetFastestResults()
	{
		//arrange
		var course = new Course { Results = CourseData.Results, Distance = new Distance("10K") };

		//act
		var fastest = course.Fastest();

		//assert
		Assert.Equal(4, fastest.Count);
		Assert.Equal(CourseData.Athlete2, fastest[0].Result.Athlete);
		Assert.Equal(CourseData.Athlete3, fastest[1].Result.Athlete);
		Assert.Equal(CourseData.Athlete1, fastest[2].Result.Athlete);
		Assert.Equal(CourseData.Athlete4, fastest[3].Result.Athlete);
	}

	[Fact]
	public void CanGetFastestResultsForCategory()
	{
		//arrange
		var course = new Course { Results = CourseData.Results, Distance = new Distance("10K") };

		//act
		var fastest = course.Fastest(Filter.F);

		//assert
		Assert.Equal(2, fastest.Count);
		Assert.Equal(CourseData.Athlete2, fastest[0].Result.Athlete);
		Assert.Equal(CourseData.Athlete4, fastest[1].Result.Athlete);
	}

	[Fact]
	public void CanGetMostRuns()
	{
		//arrange
		var course = new Course { Results = CourseData.Results, Distance = new Distance("10K") };

		//act
		var most = course.MostRuns();

		//assert
		Assert.Equal(4, most.Count);
		Assert.Equal(CourseData.Athlete4, most[0].Result.Athlete);
		Assert.Equal(CourseData.Athlete3, most[1].Result.Athlete);
		Assert.Equal(CourseData.Athlete1, most[2].Result.Athlete);
		Assert.Equal(CourseData.Athlete2, most[3].Result.Athlete);
	}

	[Fact]
	public void CanGetMostRunsForCategory()
	{
		//arrange
		var course = new Course { Results = CourseData.Results, Distance = new Distance("10K") };

		//act
		var most = course.MostRuns(Filter.M);

		//assert
		Assert.Equal(2, most.Count);
		Assert.Equal(CourseData.Athlete3, most[0].Result.Athlete);
		Assert.Equal(CourseData.Athlete1, most[1].Result.Athlete);
	}

	[Fact]
	public void CanGetMostMiles()
	{
		//arrange
		var course = new Course { Results = CourseData.Results, Distance = new Distance("2 miles") };

		//act
		var most = course.MostMiles();

		//assert
		Assert.Equal(4, most.Count);
		Assert.Equal(CourseData.Athlete4, most[0].Result.Athlete);
		Assert.Equal(CourseData.Athlete3, most[1].Result.Athlete);
		Assert.Equal(CourseData.Athlete1, most[2].Result.Athlete);
		Assert.Equal(CourseData.Athlete2, most[3].Result.Athlete);
	}

	[Fact]
	public void CanGetMostMilesForCategory()
	{
		//arrange
		var course = new Course { Results = CourseData.Results, Distance = new Distance("10K") };

		//act
		var most = course.MostMiles(Filter.M);

		//assert
		Assert.Equal(2, most.Count);
		Assert.Equal(CourseData.Athlete3, most[0].Result.Athlete);
		Assert.Equal(CourseData.Athlete1, most[1].Result.Athlete);
	}

	[Fact]
	public void CanGetBestAverage()
	{
		//arrange
		var course = new Course { Results = CourseData.Results, Distance = new Distance("10K") };

		//act
		var avg = course.BestAverage();

		//assert
		Assert.Equal(3, avg.Count);
		Assert.Equal(CourseData.Athlete3, avg[0].Result.Athlete);
		Assert.Equal(CourseData.Athlete1, avg[1].Result.Athlete);
		Assert.Equal(CourseData.Athlete4, avg[2].Result.Athlete);
	}

	[Fact]
	public void CanGetBestAverageForCategory()
	{
		//arrange
		var course = new Course { Results = CourseData.Results, Distance = new Distance("10K") };

		//act
		var avg = course.BestAverage(Filter.M);

		//assert
		Assert.Equal(2, avg.Count);
		Assert.Equal(CourseData.Athlete3, avg[0].Result.Athlete);
		Assert.Equal(CourseData.Athlete1, avg[1].Result.Athlete);
	}

	[Fact]
	public void BestAverageDropsAthletesBelowThreshold()
	{
		//arrange
		var course = new Course { Results = CourseData.Results, Distance = new Distance("10K") };

		//act
		var avg = course.BestAverage(Filter.F);

		//assert
		Assert.Single(avg);
		Assert.Equal(CourseData.Athlete4, avg[0].Result.Athlete);
	}

	[Fact]
	public void CanGetEarliestRun()
	{
		//arrange
		var course = new Course { Results = CourseData.Results, Distance = new Distance("10K") };

		//act
		var earliest = course.Earliest();

		//assert
		Assert.Equal(4, earliest.Count);
		Assert.Equal(CourseData.Athlete1, earliest[0].Result.Athlete);
		Assert.Equal(CourseData.Athlete3, earliest[1].Result.Athlete);
		Assert.Equal(CourseData.Athlete4, earliest[2].Result.Athlete);
		Assert.Equal(CourseData.Athlete2, earliest[3].Result.Athlete);
	}

	[Fact]
	public void EarliestRunIsBasedOnFinishTime()
	{
		//arrange
		var course = new Course
		{
			Results =
			[
				new Result { Athlete = CourseData.Athlete1, Course = CourseData.Course, StartTime = new Date(DateTime.Parse("4/15/2024 9:40am")), Duration = new Time(TimeSpan.FromHours(3))},
				new Result { Athlete = CourseData.Athlete2, Course = CourseData.Course, StartTime = new Date(DateTime.Parse("4/15/2024 9:45am")), Duration = new Time(TimeSpan.FromHours(2))},
				new Result { Athlete = CourseData.Athlete3, Course = CourseData.Course, StartTime = new Date(DateTime.Parse("4/15/2024 11:50am")), Duration = new Time(TimeSpan.FromHours(1))}
			],
			Distance = new Distance("10K")
		};

		//act
		var earliest = course.Earliest();

		//assert
		Assert.Equal(3, earliest.Count);
		Assert.Equal(CourseData.Athlete2, earliest[0].Result.Athlete);
		Assert.Equal(CourseData.Athlete1, earliest[1].Result.Athlete);
		Assert.Equal(CourseData.Athlete3, earliest[2].Result.Athlete);
	}

	[Fact]
	public void CanGetEarliestForCategory()
	{
		//arrange
		var course = new Course { Results = CourseData.Results, Distance = new Distance("10K") };

		//act
		var earliest = course.Earliest(Filter.M);

		//assert
		Assert.Equal(2, earliest.Count);
		Assert.Equal(CourseData.Athlete1, earliest[0].Result.Athlete);
		Assert.Equal(CourseData.Athlete3, earliest[1].Result.Athlete);
	}

	[Fact]
	public void CanGetCommunityStars()
	{
		//arrange
		var course = new Course { Distance = new Distance("10K") };
		course.Results =
		[
			new Result { Course = course, Athlete = CourseData.Athlete1, StartTime = new Date(new DateTime(2022, 4, 8)), Duration = new Time(TimeSpan.FromHours(1)), CommunityStars = { [StarType.GroupRun] = true, [StarType.Story] = true } },
			new Result { Course = course, Athlete = CourseData.Athlete3, StartTime = new Date(new DateTime(2022, 4, 8)), Duration = new Time(TimeSpan.FromHours(1)), CommunityStars = { [StarType.GroupRun] = true, [StarType.Story] = true } },
			new Result { Course = course, Athlete = CourseData.Athlete4, StartTime = new Date(new DateTime(2022, 4, 8)), Duration = new Time(TimeSpan.FromHours(1)), CommunityStars = { [StarType.GroupRun] = true } },
			new Result { Course = course, Athlete = CourseData.Athlete2, StartTime = new Date(new DateTime(2022, 4, 7)), Duration = new Time(TimeSpan.FromHours(1)) }
		];

		//act
		var points = course.CommunityStars();

		//assert
		Assert.Equal(3, points.Count);
		Assert.Equal(2, points[0].Value.Value);
		Assert.Equal(2, points[1].Value.Value);
		Assert.Equal(1, points[2].Value.Value);
		Assert.Equal(CourseData.Athlete1, points[0].Result.Athlete);
		Assert.Equal(CourseData.Athlete3, points[1].Result.Athlete);
		Assert.Equal(CourseData.Athlete4, points[2].Result.Athlete);
	}

	[Fact]
	public void CanGetCommunityStarsForCategory()
	{
		//arrange
		var course = new Course { Distance = new Distance("10K") };
		course.Results =
		[
			new Result { Course = course, Athlete = CourseData.Athlete1, StartTime = new Date(new DateTime(2022, 4, 8)), Duration = new Time(TimeSpan.FromHours(1)), CommunityStars = { [StarType.GroupRun] = true, [StarType.Story] = true } },
			new Result { Course = course, Athlete = CourseData.Athlete3, StartTime = new Date(new DateTime(2022, 4, 8)), Duration = new Time(TimeSpan.FromHours(1)), CommunityStars = { [StarType.GroupRun] = true } },
			new Result { Course = course, Athlete = CourseData.Athlete4, StartTime = new Date(new DateTime(2022, 4, 8)), Duration = new Time(TimeSpan.FromHours(1)), CommunityStars = { [StarType.GroupRun] = true, [StarType.Story] = true } },
			new Result { Course = course, Athlete = CourseData.Athlete2, StartTime = new Date(new DateTime(2022, 4, 7)), Duration = new Time(TimeSpan.FromHours(1)) }
		];

		//act
		var points = course.CommunityStars(Filter.M);

		//assert
		Assert.Equal(2, points.Count);
		Assert.Equal(2, points[0].Value.Value);
		Assert.Equal(1, points[1].Value.Value);
		Assert.Equal(CourseData.Athlete1, points[0].Result.Athlete);
		Assert.Equal(CourseData.Athlete3, points[1].Result.Athlete);
	}

	[Fact]
	public void CanGetPointsForFastestTime()
	{
		//arrange
		var course = new Course { Results = CourseData.Results, Distance = new Distance("10K") };

		//act
		var results = course.Fastest();

		//assert
		Assert.Equal(100, results.First().Points.Value);
	}

	[Fact]
	public void CanGetPointsBasedOnFastestTime()
	{
		//arrange
		var course = new Course { Results = CourseData.Results, Distance = new Distance("10K") };

		//act
		var results = course.Fastest(Filter.M);

		//assert
		Assert.Equal(75, results.Skip(1).First().Points.Value);
	}

	[Fact]
	public void AverageAgeGradeBasedOnIndividualFastestTime()
	{
		//arrange
		var course = new Course { Results = CourseData.Results, Distance = new Distance("10K") };

		//act
		var results = course.TeamPoints();

		//assert
		Assert.Equal("8.76%", results.First().Value.AverageAgeGrade.Display);
	}

	[Fact]
	public void CanGetStatisticsForCourse()
	{
		//arrange
		var course = new Course { Results = CourseData.Results, Distance = new Distance("10K") };

		//act
		var stats = course.Statistics();

		//assert
		Assert.Equal(4, stats.Participants[string.Empty]);
		Assert.Equal(8, stats.Runs[string.Empty]);
		Assert.Equal(8 * 10000 / Distance.MetersPerMile, stats.Miles[string.Empty]);
		Assert.Equal(2, stats.Average[string.Empty]);
	}

	[Fact]
	public void RanksWithTiesSkipCorrectly()
	{
		//arrange
		var courseResults = CourseData.Results.ToList();
		courseResults.Add(new Result
		{
			Course = CourseData.Course,
			Athlete = CourseData.Athlete1,
			StartTime = new Date(DateTime.Parse("1/9/2020")),
			Duration = new Time(TimeSpan.Parse("0:54:32.1"))
		});
		var course = new Course { Results = courseResults.ToArray(), Distance = new Distance("10K") };

		//act
		var results = course.Fastest();

		//assert
		Assert.Equal(1, results[0].Rank.Value);
		Assert.Equal(1, results[1].Rank.Value);
		Assert.Equal(3, results[2].Rank.Value);
	}

	[Fact]
	public void GroupedResultsFiltersByAgeGroup()
	{
		//arrange
		var results = new[]
		{
			new Result { Athlete = CourseData.Athlete3, StartTime = new Date(new DateTime(2019, 12, 31)), Duration = new Time(new TimeSpan(1, 2, 3)) },
			new Result { Athlete = CourseData.Athlete3, StartTime = new Date(new DateTime(2020, 1, 2)), Duration = new Time(new TimeSpan(2, 3, 4)) },
			new Result { Athlete = CourseData.Athlete4, StartTime = new Date(new DateTime(2019, 12, 31)), Duration = new Time(new TimeSpan(3, 2, 1)) },
			new Result { Athlete = CourseData.Athlete4, StartTime = new Date(new DateTime(2020, 1, 2)), Duration = new Time(new TimeSpan(4, 3, 2)) }
		};
		var course = new Course { Results = results };

		//act
		var m2 = course.GroupedResults(new Filter(Category.M, Athlete.Teams[2]));
		var m3 = course.GroupedResults(new Filter(Category.M, Athlete.Teams[3]));
		var f2 = course.GroupedResults(new Filter(Category.F, Athlete.Teams[2]));
		var f3 = course.GroupedResults(new Filter(Category.F, Athlete.Teams[3]));

		//assert
		Assert.Equal(new TimeSpan(1, 2, 3), m2.First().First().Duration.Value);
		Assert.Equal(new TimeSpan(2, 3, 4), m3.First().First().Duration.Value);
		Assert.Equal(new TimeSpan(3, 2, 1), f2.First().First().Duration.Value);
		Assert.Equal(new TimeSpan(4, 3, 2), f3.First().First().Duration.Value);
	}

	[Fact]
	public void PrivateAthletesAreIncludedInFastestBecauseTeamMembersNeedsIt()
	{
		//arrange
		var results = new[]
		{
			new Result { Athlete = CourseData.Private, StartTime = new Date(new DateTime(2023, 1, 1)) }
		};
		var course = new Course { Results = results };

		//act
		var fastest = course.Fastest();

		//assert
		Assert.Equal(CourseData.Private, fastest.First().Result.Athlete);
		Assert.Equal(Time.Max, fastest.First().Value);
		Assert.Null(fastest.First().Result.Duration);
	}

	[Fact]
	public void PrivateAthletesAreIncludedInMostMiles()
	{
		//arrange
		var results = new[]
		{
			new Result { Athlete = CourseData.Private, StartTime = new Date(new DateTime(2023, 1, 1)) }
		};
		var course = new Course { Results = results, Distance = new Distance("10K") };

		//act
		var most = course.MostMiles();

		//assert
		Assert.Equal(CourseData.Private, most.First().Result.Athlete);
	}

	[Fact]
	public void PrivateAthletesAreIncludedInMostRuns()
	{
		//arrange
		var results = new[]
		{
			new Result { Athlete = CourseData.Private, StartTime = new Date(new DateTime(2023, 1, 1)) }
		};
		var course = new Course { Results = results };

		//act
		var most = course.MostRuns();

		//assert
		Assert.Equal(CourseData.Private, most.First().Result.Athlete);
	}

	[Fact]
	public void PrivateAthletesAreIncludedInCommunityStars()
	{
		//arrange
		var results = new[]
		{
			new Result { Athlete = CourseData.Private, StartTime = new Date(new DateTime(2023, 1, 1)), CommunityStars = { [StarType.Story] = true }}
		};
		var course = new Course { Results = results };

		//act
		var stars = course.CommunityStars();

		//assert
		Assert.Equal(CourseData.Private, stars.First().Result.Athlete);
	}

	[Fact]
	public void PrivateAthletesAreNotIncludedInBestAverage()
	{
		//arrange
		var results = new[]
		{
			new Result { Athlete = CourseData.Private, StartTime = new Date(new DateTime(2023, 1, 1)) }
		};
		var course = new Course { Results = results };

		//act
		var best = course.BestAverage();

		//assert
		Assert.Empty(best);
	}

	[Fact] public void PrivateAthleteCannotBeInFirstForFastestButNeedsToBeThereForTeamMembers()
	{

		//arrange
		var results = new[]
		{
			new Result { Athlete = CourseData.Athlete1, Course = CourseData.Course, StartTime = new Date(new DateTime(2023, 1, 1)), Duration = new Time(TimeSpan.FromHours(1)) },
			new Result { Athlete = CourseData.Private, Course = CourseData.Course, StartTime = new Date(new DateTime(2023, 1, 1)) },
			new Result { Athlete = CourseData.Private, Course = CourseData.Course, StartTime = new Date(new DateTime(2023, 1, 2)) }
		};
		var course = new Course { Results = results, Distance = new Distance("10K")};

		//act
		var fastest = course.Fastest();

		//assert
		Assert.Equal(1, fastest[0].Rank.Value);
		Assert.Equal(CourseData.Athlete1, fastest[0].Result.Athlete);
		Assert.Equal(2, fastest[1].Rank.Value);
		Assert.Equal(CourseData.Private, fastest[1].Result.Athlete);
	}

	[Fact] public void PrivateAthleteCannotBeInFirstForBestAverage()
	{

		//arrange
		var results = new[]
		{
			new Result { Athlete = CourseData.Athlete1, Course = CourseData.Course, StartTime = new Date(new DateTime(2023, 1, 1)), Duration = new Time(TimeSpan.FromHours(1)) },
			new Result { Athlete = CourseData.Athlete1, Course = CourseData.Course, StartTime = new Date(new DateTime(2023, 1, 1)), Duration = new Time(TimeSpan.FromHours(1)) },
			new Result { Athlete = CourseData.Private, Course = CourseData.Course, StartTime = new Date(new DateTime(2023, 1, 1)) },
			new Result { Athlete = CourseData.Private, Course = CourseData.Course, StartTime = new Date(new DateTime(2023, 1, 2)) }
		};
		var course = new Course { Results = results, Distance = new Distance("10K")};

		//act
		var best = course.BestAverage();

		//assert
		Assert.Single(best);
		Assert.Equal(1, best[0].Rank.Value);
		Assert.Equal(CourseData.Athlete1, best[0].Result.Athlete);
	}

	[Fact] public void PrivateAthleteRunCountContributesTowardsBestAverageMinimum()
	{

		//arrange
		var results = new[]
		{
			new Result { Athlete = CourseData.Athlete1, Course = CourseData.Course, StartTime = new Date(new DateTime(2023, 1, 1)), Duration = new Time(TimeSpan.FromHours(1)) },
			new Result { Athlete = CourseData.Private, Course = CourseData.Course, StartTime = new Date(new DateTime(2023, 1, 1)) },
			new Result { Athlete = CourseData.Private, Course = CourseData.Course, StartTime = new Date(new DateTime(2023, 1, 2)) }
		};
		var course = new Course { Results = results, Distance = new Distance("10K")};

		//act
		var best = course.BestAverage();

		//assert
		Assert.Empty(best);
	}

	[Fact] public void PrivateAthleteCanBeInFirstForMostMiles()
	{

		//arrange
		var results = new[]
		{
			new Result { Athlete = CourseData.Athlete1, StartTime = new Date(new DateTime(2023, 1, 1)), Duration = new Time(TimeSpan.FromHours(1)), CommunityStars = { [StarType.GroupRun] = true } },
			new Result { Athlete = CourseData.Private, StartTime = new Date(new DateTime(2023, 1, 1)) },
			new Result { Athlete = CourseData.Private, StartTime = new Date(new DateTime(2023, 1, 2)) }
		};
		var course = new Course { Results = results, Distance = new Distance("10K")};

		//act
		var miles = course.MostMiles();

		//assert
		Assert.Equal(1, miles[0].Rank.Value);
		Assert.Equal(CourseData.Private, miles[0].Result.Athlete);
		Assert.Equal(2, miles[1].Rank.Value);
		Assert.Equal(CourseData.Athlete1, miles[1].Result.Athlete);
	}

	[Fact]
	public void PrivateAthleteCanBeInFirstForMostRuns()
	{

		//arrange
		var results = new[]
		{
			new Result { Athlete = CourseData.Athlete1, StartTime = new Date(new DateTime(2023, 1, 1)), Duration = new Time(TimeSpan.FromHours(1)), CommunityStars = { [StarType.GroupRun] = true } },
			new Result { Athlete = CourseData.Private, StartTime = new Date(new DateTime(2023, 1, 1)) },
			new Result { Athlete = CourseData.Private, StartTime = new Date(new DateTime(2023, 1, 2)) }
		};
		var course = new Course { Results = results, Distance = new Distance("10K")};

		//act
		var runs = course.MostRuns();

		//assert
		Assert.Equal(1, runs[0].Rank.Value);
		Assert.Equal(CourseData.Private, runs[0].Result.Athlete);
		Assert.Equal(2, runs[1].Rank.Value);
		Assert.Equal(CourseData.Athlete1, runs[1].Result.Athlete);
	}

	[Fact]
	public void PrivateAthleteCanBeInFirstForCommunityStars()
	{

		//arrange
		var results = new[]
		{
			new Result { Athlete = CourseData.Athlete1, StartTime = new Date(new DateTime(2023, 1, 1)), Duration = new Time(TimeSpan.FromHours(1)), CommunityStars = { [StarType.GroupRun] = true } },
			new Result { Athlete = CourseData.Private, StartTime = new Date(new DateTime(2023, 1, 1)), CommunityStars = { [StarType.Story] = true, [StarType.GroupRun] = true } }
		};
		var course = new Course { Results = results, Distance = new Distance("10K")};

		//act
		var stars = course.CommunityStars();

		//assert
		Assert.Equal(1, stars[0].Rank.Value);
		Assert.Equal(CourseData.Private, stars[0].Result.Athlete);
		Assert.Equal(2, stars[1].Rank.Value);
		Assert.Equal(CourseData.Athlete1, stars[1].Result.Athlete);
	}
}