using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Model;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests;

public sealed class ResultsExtensionsTests
{
	[Fact]
	public void CanGetFastestResults()
	{
		//arrange
		var results = ResultsData.Results;

		//act
		var fastest = results.Fastest();

		//assert
		Assert.Equal(4, fastest.Count);
		Assert.Equal(ResultsData.Athlete2, fastest[0].Result.Athlete);
		Assert.Equal(ResultsData.Athlete3, fastest[1].Result.Athlete);
		Assert.Equal(ResultsData.Athlete1, fastest[2].Result.Athlete);
		Assert.Equal(ResultsData.Athlete4, fastest[3].Result.Athlete);
	}

	[Fact]
	public void CanGetFastestResultsForCategory()
	{
		//arrange
		var results = ResultsData.Results;

		//act
		var fastest = results.Fastest(new Filter(Category.F));

		//assert
		Assert.Equal(2, fastest.Count);
		Assert.Equal(ResultsData.Athlete2, fastest[0].Result.Athlete);
		Assert.Equal(ResultsData.Athlete4, fastest[1].Result.Athlete);
	}

	[Fact]
	public void CanGetMostRuns()
	{
		//arrange
		var results = ResultsData.Results;

		//act
		var most = results.MostRuns();

		//assert
		Assert.Equal(4, most.Count);
		Assert.Equal(ResultsData.Athlete4, most[0].Result.Athlete);
		Assert.Equal(ResultsData.Athlete3, most[1].Result.Athlete);
		Assert.Equal(ResultsData.Athlete1, most[2].Result.Athlete);
		Assert.Equal(ResultsData.Athlete2, most[3].Result.Athlete);
	}

	[Fact]
	public void CanGetMostRunsForCategory()
	{
		//arrange
		var results = ResultsData.Results;

		//act
		var most = results.MostRuns(new Filter(Category.M));

		//assert
		Assert.Equal(2, most.Count);
		Assert.Equal(ResultsData.Athlete3, most[0].Result.Athlete);
		Assert.Equal(ResultsData.Athlete1, most[1].Result.Athlete);
	}

	[Fact]
	public void CanGetMostMiles()
	{
		//arrange
		var results = ResultsData.Results;

		//act
		var most = results.MostMiles();

		//assert
		Assert.Equal(4, most.Count);
		Assert.Equal(ResultsData.Athlete4, most[0].Result.Athlete);
		Assert.Equal(ResultsData.Athlete3, most[1].Result.Athlete);
		Assert.Equal(ResultsData.Athlete1, most[2].Result.Athlete);
		Assert.Equal(ResultsData.Athlete2, most[3].Result.Athlete);
	}

	[Fact]
	public void CanGetMostMilesForCategory()
	{
		//arrange
		var results = ResultsData.Results;

		//act
		var most = results.MostMiles(new Filter(Category.M));

		//assert
		Assert.Equal(2, most.Count);
		Assert.Equal(ResultsData.Athlete3, most[0].Result.Athlete);
		Assert.Equal(ResultsData.Athlete1, most[1].Result.Athlete);
	}

	[Fact]
	public void CanGetBestAverage()
	{
		//arrange
		var results = ResultsData.Results;

		//act
		var avg = results.BestAverage();

		//assert
		Assert.Equal(3, avg.Count);
		Assert.Equal(ResultsData.Athlete3, avg[0].Result.Athlete);
		Assert.Equal(ResultsData.Athlete1, avg[1].Result.Athlete);
		Assert.Equal(ResultsData.Athlete4, avg[2].Result.Athlete);
	}

	[Fact]
	public void CanGetBestAverageForCategory()
	{
		//arrange
		var results = ResultsData.Results;

		//act
		var avg = results.BestAverage(new Filter(Category.M));

		//assert
		Assert.Equal(2, avg.Count);
		Assert.Equal(ResultsData.Athlete3, avg[0].Result.Athlete);
		Assert.Equal(ResultsData.Athlete1, avg[1].Result.Athlete);
	}

	[Fact]
	public void BestAverageDropsAthletesBelowThreshold()
	{
		//arrange
		var results = ResultsData.Results;

		//act
		var avg = results.BestAverage(new Filter(Category.F));

		//assert
		Assert.Single(avg);
		Assert.Equal(ResultsData.Athlete4, avg[0].Result.Athlete);
	}

	[Fact]
	public void CanGetEarliestRun()
	{
		//arrange
		var results = ResultsData.Results;

		//act
		var earliest = results.Earliest();

		//assert
		Assert.Equal(4, earliest.Count);
		Assert.Equal(ResultsData.Athlete1, earliest[0].Result.Athlete);
		Assert.Equal(ResultsData.Athlete3, earliest[1].Result.Athlete);
		Assert.Equal(ResultsData.Athlete4, earliest[2].Result.Athlete);
		Assert.Equal(ResultsData.Athlete2, earliest[3].Result.Athlete);
	}

	[Fact]
	public void EarliestRunIsBasedOnFinishTime()
	{
		//arrange
		var results = new[]
		{
			new Result { Athlete = ResultsData.Athlete1, Course = ResultsData.Course, StartTime = DateTime.Parse("4/15/2024 9:40am"), Duration = TimeSpan.FromHours(3) },
			new Result { Athlete = ResultsData.Athlete2, Course = ResultsData.Course, StartTime = DateTime.Parse("4/15/2024 9:45am"), Duration = TimeSpan.FromHours(2) },
			new Result { Athlete = ResultsData.Athlete3, Course = ResultsData.Course, StartTime = DateTime.Parse("4/15/2024 11:50am"), Duration = TimeSpan.FromHours(1) }
		};

		//act
		var earliest = results.Earliest();

		//assert
		Assert.Equal(3, earliest.Count);
		Assert.Equal(ResultsData.Athlete2, earliest[0].Result.Athlete);
		Assert.Equal(ResultsData.Athlete1, earliest[1].Result.Athlete);
		Assert.Equal(ResultsData.Athlete3, earliest[2].Result.Athlete);
	}

	[Fact]
	public void CanGetEarliestForCategory()
	{
		//arrange
		var results = ResultsData.Results;

		//act
		var earliest = results.Earliest(new Filter(Category.M));

		//assert
		Assert.Equal(2, earliest.Count);
		Assert.Equal(ResultsData.Athlete1, earliest[0].Result.Athlete);
		Assert.Equal(ResultsData.Athlete3, earliest[1].Result.Athlete);
	}

	[Fact]
	public void CanGetPointsForFastestTime()
	{
		//arrange
		var results = ResultsData.Results;

		//act
		var fastest = results.Fastest();

		//assert
		Assert.Equal(100, fastest.First().Points.Value);
	}

	[Fact]
	public void CanGetPointsBasedOnFastestTime()
	{
		//arrange
		var results = ResultsData.Results;

		//act
		var fastest = results.Fastest(new Filter(Category.M));

		//assert
		Assert.Equal(75, fastest.Skip(1).First().Points.Value);
	}

	[Fact]
	public void RanksWithTiesSkipCorrectly()
	{
		//arrange
		var results = ResultsData.Results.ToList();
		results.Add(new Result
		{
			Course = ResultsData.Course,
			Athlete = ResultsData.Athlete1,
			StartTime = DateTime.Parse("1/9/2020"),
			Duration = TimeSpan.Parse("0:54:32.1")
		});

		//act
		var fastest = results.Fastest();

		//assert
		Assert.Equal(1, fastest[0].Rank.Value);
		Assert.Equal(1, fastest[1].Rank.Value);
		Assert.Equal(3, fastest[2].Rank.Value);
	}

	[Fact]
	public void GroupedResultsFiltersByAgeGroup()
	{
		//arrange
		var results = new[]
		{
			new Result { Athlete = ResultsData.Athlete3, StartTime = new DateTime(2019, 12, 31), Duration = new TimeSpan(1, 2, 3) },
			new Result { Athlete = ResultsData.Athlete3, StartTime = new DateTime(2020, 1, 2), Duration = new TimeSpan(2, 3, 4) },
			new Result { Athlete = ResultsData.Athlete4, StartTime = new DateTime(2019, 12, 31), Duration = new TimeSpan(3, 2, 1) },
			new Result { Athlete = ResultsData.Athlete4, StartTime = new DateTime(2020, 1, 2), Duration = new TimeSpan(4, 3, 2) }
		};

		//act
		var m2 = results.GroupedResults(new Filter(Category.M, Team.Teams[2]));
		var m3 = results.GroupedResults(new Filter(Category.M, Team.Teams[3]));
		var f2 = results.GroupedResults(new Filter(Category.F, Team.Teams[2]));
		var f3 = results.GroupedResults(new Filter(Category.F, Team.Teams[3]));

		//assert
		Assert.Equal(new TimeSpan(1, 2, 3), m2.First().First().Duration);
		Assert.Equal(new TimeSpan(2, 3, 4), m3.First().First().Duration);
		Assert.Equal(new TimeSpan(3, 2, 1), f2.First().First().Duration);
		Assert.Equal(new TimeSpan(4, 3, 2), f3.First().First().Duration);
	}

	[Fact]
	public void PrivateAthletesAreIncludedInFastestBecauseTeamMembersNeedsIt()
	{
		//arrange
		var results = new[]
		{
			new Result { Athlete = ResultsData.Private, StartTime = new DateTime(2023, 1, 1), Course = new Course() }
		};

		//act
		var fastest = results.Fastest();

		//assert
		Assert.Equal(ResultsData.Private, fastest.First().Result.Athlete);
		Assert.Equal(Time.Max, fastest.First().Value);
		Assert.Equal(TimeSpan.Zero, fastest.First().Result.Duration);
	}

	[Fact]
	public void PrivateAthletesAreIncludedInMostMiles()
	{
		//arrange
		var results = new[]
		{
			new Result
			{
				Course = ResultsData.Course,
				Athlete = ResultsData.Private,
				StartTime = new DateTime(2023, 1, 1)
			}
		};

		//act
		var most = results.MostMiles();

		//assert
		Assert.Equal(ResultsData.Private, most.First().Result.Athlete);
	}

	[Fact]
	public void PrivateAthletesAreIncludedInMostRuns()
	{
		//arrange
		var results = new[]
		{
			new Result { Athlete = ResultsData.Private, StartTime = new DateTime(2023, 1, 1) }
		};

		//act
		var most = results.MostRuns();

		//assert
		Assert.Equal(ResultsData.Private, most.First().Result.Athlete);
	}

	[Fact]
	public void PrivateAthletesAreNotIncludedInBestAverage()
	{
		//arrange
		var results = new[]
		{
			new Result { Athlete = ResultsData.Private, StartTime = new DateTime(2023, 1, 1) }
		};

		//act
		var best = results.BestAverage();

		//assert
		Assert.Empty(best);
	}

	[Fact]
	public void PrivateAthleteCannotBeInFirstForFastestButNeedsToBeThereForTeamMembers()
	{
		//arrange
		var results = new[]
		{
			new Result { Athlete = ResultsData.Athlete1, Course = ResultsData.Course, StartTime = new DateTime(2023, 1, 1), Duration = TimeSpan.FromHours(1) },
			new Result { Athlete = ResultsData.Private, Course = ResultsData.Course, StartTime = new DateTime(2023, 1, 1) },
			new Result { Athlete = ResultsData.Private, Course = ResultsData.Course, StartTime = new DateTime(2023, 1, 2) }
		};

		//act
		var fastest = results.Fastest();

		//assert
		Assert.Equal(1, fastest[0].Rank.Value);
		Assert.Equal(ResultsData.Athlete1, fastest[0].Result.Athlete);
		Assert.Equal(2, fastest[1].Rank.Value);
		Assert.Equal(ResultsData.Private, fastest[1].Result.Athlete);
	}

	[Fact]
	public void PrivateAthleteCannotBeInFirstForBestAverage()
	{
		//arrange
		var results = new[]
		{
			new Result { Athlete = ResultsData.Athlete1, Course = ResultsData.Course, StartTime = new DateTime(2023, 1, 1), Duration = TimeSpan.FromHours(1) },
			new Result { Athlete = ResultsData.Athlete1, Course = ResultsData.Course, StartTime = new DateTime(2023, 1, 1), Duration = TimeSpan.FromHours(1) },
			new Result { Athlete = ResultsData.Private, Course = ResultsData.Course, StartTime = new DateTime(2023, 1, 1) },
			new Result { Athlete = ResultsData.Private, Course = ResultsData.Course, StartTime = new DateTime(2023, 1, 2) }
		};

		//act
		var best = results.BestAverage();

		//assert
		Assert.Single(best);
		Assert.Equal(1, best[0].Rank.Value);
		Assert.Equal(ResultsData.Athlete1, best[0].Result.Athlete);
	}

	[Fact]
	public void PrivateAthleteRunCountContributesTowardsBestAverageMinimum()
	{
		//arrange
		var results = new[]
		{
			new Result { Athlete = ResultsData.Athlete1, Course = ResultsData.Course, StartTime = new DateTime(2023, 1, 1), Duration = TimeSpan.FromHours(1) },
			new Result { Athlete = ResultsData.Private, Course = ResultsData.Course, StartTime = new DateTime(2023, 1, 1) },
			new Result { Athlete = ResultsData.Private, Course = ResultsData.Course, StartTime = new DateTime(2023, 1, 2) }
		};

		//act
		var best = results.BestAverage();

		//assert
		Assert.Empty(best);
	}

	[Fact]
	public void PrivateAthleteCanBeInFirstForMostMiles()
	{
		//arrange
		var results = new[]
		{
			new Result { Course = ResultsData.Course, Athlete = ResultsData.Athlete1, StartTime = new DateTime(2023, 1, 1), Duration = TimeSpan.FromHours(1) },
			new Result { Course = ResultsData.Course, Athlete = ResultsData.Private, StartTime = new DateTime(2023, 1, 1) },
			new Result { Course = ResultsData.Course, Athlete = ResultsData.Private, StartTime = new DateTime(2023, 1, 2) }
		};

		//act
		var miles = results.MostMiles();

		//assert
		Assert.Equal(1, miles[0].Rank.Value);
		Assert.Equal(ResultsData.Private, miles[0].Result.Athlete);
		Assert.Equal(2, miles[1].Rank.Value);
		Assert.Equal(ResultsData.Athlete1, miles[1].Result.Athlete);
	}

	[Fact]
	public void PrivateAthleteCanBeInFirstForMostRuns()
	{
		//arrange
		var results = new[]
		{
			new Result { Course = ResultsData.Course, Athlete = ResultsData.Athlete1, StartTime = new DateTime(2023, 1, 1), Duration = TimeSpan.FromHours(1) },
			new Result { Course = ResultsData.Course, Athlete = ResultsData.Private, StartTime = new DateTime(2023, 1, 1) },
			new Result { Course = ResultsData.Course, Athlete = ResultsData.Private, StartTime = new DateTime(2023, 1, 2) }
		};

		//act
		var runs = results.MostRuns();

		//assert
		Assert.Equal(1, runs[0].Rank.Value);
		Assert.Equal(ResultsData.Private, runs[0].Result.Athlete);
		Assert.Equal(2, runs[1].Rank.Value);
		Assert.Equal(ResultsData.Athlete1, runs[1].Result.Athlete);
	}

	[Fact]
	public void CanGetStatisticsForCourse()
	{
		//arrange
		var results = ResultsData.Results;

		//act
		var stats = results.Statistics();

		//assert
		Assert.Equal(4, stats.Participants[string.Empty]);
		Assert.Equal(8, stats.Runs[string.Empty]);
		Assert.Equal(8 * 10000 / Core.Races.Distance.MetersPerMile, stats.Miles[string.Empty]);
		Assert.Equal(2, stats.Average[string.Empty]);
	}
}