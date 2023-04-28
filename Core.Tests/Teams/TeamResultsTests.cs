using FLRC.Leaderboards.Core.Teams;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Teams;

public sealed class TeamResultsTests
{
	[Fact]
	public void CanGetTotalPoints()
	{
		//arrange
		var results = new TeamResults
		{
			AgeGradePoints = 5,
			MostRunsPoints = 6
		};

		//act
		var total = results.TotalPoints;

		//assert
		Assert.Equal(11, total);
	}

	[Fact]
	public void DisplayedValueIsTotalPoints()
	{
		//arrange
		var results = new TeamResults
		{
			AgeGradePoints = 5,
			MostRunsPoints = 6
		};

		//act
		var display = results.Display;

		//assert
		Assert.Equal("11", display);
	}
}