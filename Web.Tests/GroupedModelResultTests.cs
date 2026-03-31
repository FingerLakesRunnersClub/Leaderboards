using FLRC.Leaderboards.Model;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests;

public sealed class GroupedModelResultTests
{
	[Fact]
	public void CanGetAverageDuration()
	{
		var athlete = new Athlete();

		//arrange
		var results = new[]
		{
			new Result { Athlete = athlete, Duration = TimeSpan.Parse("1:00") },
			new Result { Athlete = athlete, Duration = TimeSpan.Parse("1:10") },
			new Result { Athlete = athlete, Duration = TimeSpan.Zero },
			new Result { Athlete = athlete, Duration = TimeSpan.Parse("1:20") },
			new Result { Athlete = athlete, Duration = TimeSpan.Parse("1:30") }
		};

		var groupedResult = new GroupedModelResult(results.GroupBy(r => r.Athlete).First());

		//act
		var avg = groupedResult.Average(new Course());

		//assert
		Assert.Equal(TimeSpan.Parse("1:15"), avg.Duration);
	}

	[Fact]
	public void AverageDurationIsZeroForPrivateAthlete()
	{
		var athlete = new Athlete { IsPrivate = true };

		//arrange
		var results = new[]
		{
			new Result { Athlete = athlete, Duration = TimeSpan.Parse("1:00") }
		};

		var groupedResult = new GroupedModelResult(results.GroupBy(r => r.Athlete).First());

		//act
		var avg = groupedResult.Average(new Course());

		//assert
		Assert.Equal(TimeSpan.Zero, avg.Duration);
	}

	[Fact]
	public void CanGetAverageFromTopAttempts()
	{
		//arrange
		var athlete = new Athlete();

		var results = new[]
		{
			new Result { Athlete = athlete, Duration = TimeSpan.Parse("1:00") },
			new Result { Athlete = athlete, Duration = TimeSpan.Parse("1:10") },
			new Result { Athlete = athlete, Duration = TimeSpan.Parse("1:20") }
		};

		var groupedResult = new GroupedModelResult(results.GroupBy(r => r.Athlete).First());

		//act
		var avg = groupedResult.Average(new Course(), 2);

		//assert
		Assert.Equal(TimeSpan.Parse("1:05"), avg.Duration);
	}

	[Fact]
	public void CanCompareGroupedResultsByAthleteID()
	{
		//arrange
		var athlete1 = new Athlete { ID = new Guid("FBE3BF74-3D12-4F1F-A201-2726738C3CD4") };
		var athlete2 = new Athlete { ID = new Guid("E4489496-7C82-4178-A480-3DD2D26933A7") };

		var results = new[]
		{
			new Result { Athlete = athlete1, Duration = TimeSpan.Parse("1:00") },
			new Result { Athlete = athlete2, Duration = TimeSpan.Parse("1:10") }
		};

		var groups = results.GroupBy(r => r.Athlete).ToArray();

		//act
		var group1 = new GroupedModelResult(groups[0]);
		var group2 = new GroupedModelResult(groups[1]);

		//assert
		Assert.Equal(1, group1.CompareTo(group2));
		Assert.Equal(-1, group2.CompareTo(group1));
	}
}