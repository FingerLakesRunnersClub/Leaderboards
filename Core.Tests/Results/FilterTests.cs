using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Results;

public class FilterTests
{
	[Fact]
	public void AllResultsMatchByDefault()
	{
		//arrange
		var filter = new Filter();

		//act
		var matches = filter.IsMatch(new Result());

		//assert
		Assert.True(matches);
	}

	[Fact]
	public void MatchesWhenCategorySetAndMatches()
	{
		//arrange
		var result = new Result { Athlete = new Athlete { Category = Category.M } };
		var filter = new Filter(Category.M);

		//act
		var matches = filter.IsMatch(result);

		//assert
		Assert.True(matches);
	}

	[Fact]
	public void DoesNotMatchWhenCategorySetAndDoesNotMatch()
	{
		//arrange
		var result = new Result { Athlete = new Athlete { Category = Category.F } };
		var filter = new Filter(Category.M);

		//act
		var matches = filter.IsMatch(result);

		//assert
		Assert.False(matches);
	}

	[Fact]
	public void MatchesWhenAgeGroupSetAndMatches()
	{
		//arrange
		var result = new Result
		{
			Athlete = new Athlete { Category = Category.M, DateOfBirth = new DateTime(2000, 1, 1) },
			StartTime = new Date(new DateTime(2022, 4, 25))
		};
		var filter = new Filter { AgeGroup = Athlete.Teams[2] };

		//act
		var matches = filter.IsMatch(result);

		//assert
		Assert.True(matches);
	}

	[Fact]
	public void DoesNotMatchWhenAgeGroupSetAndDoesNotMatch()
	{
		//arrange
		var result = new Result
		{
			Athlete = new Athlete { Category = Category.M, DateOfBirth = new DateTime(1990, 1, 1) },
			StartTime = new Date(new DateTime(2022, 4, 25))
		};
		var filter = new Filter { AgeGroup = Athlete.Teams[2] };

		//act
		var matches = filter.IsMatch(result);

		//assert
		Assert.False(matches);
	}
}