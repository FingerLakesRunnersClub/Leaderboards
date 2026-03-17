using Xunit;

namespace FLRC.Leaderboards.Model.Tests;

public sealed class IterationTests
{
	[Fact]
	public void IterationIsActiveWhenBetweenStartAndEndDates()
	{
		//arrange
		var iteration = new Iteration
		{
			StartDate = new DateOnly(DateTime.Today.Year, 1, 1),
			EndDate = new DateOnly(DateTime.Today.Year, 12, 31)
		};

		//act
		var isActive = iteration.IsActive;

		//assert
		Assert.True(isActive);
	}

	[Fact]
	public void IterationIsNotActiveWhenEndsInPast()
	{
		//arrange
		var iteration = new Iteration
		{
			StartDate = new DateOnly(DateTime.Today.Year - 1, 1, 1),
			EndDate = new DateOnly(DateTime.Today.Year - 1, 12, 31)
		};

		//act
		var isActive = iteration.IsActive;

		//assert
		Assert.False(isActive);
	}

	[Fact]
	public void IterationIsNotActiveWhenStartsInFuture()
	{
		//arrange
		var iteration = new Iteration
		{
			StartDate = new DateOnly(DateTime.Today.Year + 1, 1, 1),
			EndDate = new DateOnly(DateTime.Today.Year + 1, 12, 31)
		};

		//act
		var isActive = iteration.IsActive;

		//assert
		Assert.False(isActive);
	}

	[Fact]
	public void CanFindOfficialChallenge()
	{
		//arrange
		var iteration = new Iteration
		{
			Challenges =
			[
				new Challenge { Name = "c1", IsOfficial = false, IsPrimary = false },
				new Challenge { Name = "c2", IsOfficial = true, IsPrimary = false },
				new Challenge { Name = "c3", IsOfficial = true, IsPrimary = true },
				new Challenge { Name = "c4", IsOfficial = false, IsPrimary = true }
			]
		};

		//act
		var official = iteration.OfficialChallenge;

		//assert
		Assert.Equal("c3", official!.Name);
	}
}