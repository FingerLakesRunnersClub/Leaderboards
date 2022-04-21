using FLRC.Leaderboards.Core.Teams;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Teams;

public class TeamResultsExtensionsTests
{
	[Fact]
	public void CanRankResults()
	{
		//arrange
		var list = new[]
		{
			new TeamResults { AgeGradePoints = 2, MostRunsPoints = 5 },
			new TeamResults { AgeGradePoints = 3, MostRunsPoints = 6 },
			new TeamResults { AgeGradePoints = 4, MostRunsPoints = 7 }
		};

		//act
		var ranked = list.Rank();

		//assert
		Assert.Equal(1, ranked.First().Rank.Value);
		Assert.Equal(2, ranked.Skip(1).First().Rank.Value);
		Assert.Equal(3, ranked.Skip(2).First().Rank.Value);
	}
}