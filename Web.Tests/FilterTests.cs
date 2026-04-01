using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Model;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests;

public sealed class FilterTests
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
		var result = new Result { Athlete = new Athlete { Category = 'M' } };
		var filter = new Filter(Core.Athletes.Category.M);

		//act
		var matches = filter.IsMatch(result);

		//assert
		Assert.True(matches);
	}

	[Fact]
	public void DoesNotMatchWhenCategorySetAndDoesNotMatch()
	{
		//arrange
		var result = new Result { Athlete = new Athlete { Category = 'F' } };
		var filter = new Filter(Core.Athletes.Category.M);

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
			Athlete = new Athlete { Category = 'M', DateOfBirth = new DateOnly(2000, 1, 1) },
			StartTime = new DateTime(2022, 4, 25)
		};
		var filter = new Filter { AgeGroup = Team.Teams[2] };

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
			Athlete = new Athlete { Category = 'M', DateOfBirth = new DateOnly(1990, 1, 1) },
			StartTime = new DateTime(2022, 4, 25)
		};
		var filter = new Filter { AgeGroup = Team.Teams[2] };

		//act
		var matches = filter.IsMatch(result);

		//assert
		Assert.False(matches);
	}

	[Fact]
	public void MatchesWhenIterationSetAndActive()
	{
		//arrange
		var result = new Result
		{
			Athlete = new Athlete { Category = 'M', DateOfBirth = new DateOnly(2000, 1, 1) },
			StartTime = DateTime.Now
		};
		var filter = new Filter { Iteration = new Iteration { StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)) } };

		//act
		var matches = filter.IsMatch(result);

		//assert
		Assert.True(matches);
	}

	[Fact]
	public void DoesNotMatchWhenIterationSetAndInThePast()
	{
		//arrange
		var result = new Result
		{
			Athlete = new Athlete { Category = 'M', DateOfBirth = new DateOnly(1990, 1, 1) },
			StartTime = DateTime.Now
		};
		var filter = new Filter { Iteration = new Iteration { StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-2)), EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)) } };

		//act
		var matches = filter.IsMatch(result);

		//assert
		Assert.False(matches);
	}

	[Fact]
	public void DoesNotMatchWhenIterationSetAndInTheFuture()
	{
		//arrange
		var result = new Result
		{
			Athlete = new Athlete { Category = 'M', DateOfBirth = new DateOnly(1990, 1, 1) },
			StartTime = DateTime.Now
		};
		var filter = new Filter { Iteration = new Iteration { StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)), EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2)) } };

		//act
		var matches = filter.IsMatch(result);

		//assert
		Assert.False(matches);
	}
}