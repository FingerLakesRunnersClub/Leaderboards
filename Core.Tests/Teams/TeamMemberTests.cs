using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Teams;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Teams;

public sealed class TeamMemberTests
{
	[Fact]
	public void CanGetTotalsForAthlete()
	{
		//arrange
		var results = new Ranked<Time>[]
		{
			new() { Count = 1, AgeGrade = new AgeGrade(90), Result = new Result { Course = new Course { Distance = new Distance("3 miles") } } },
			new() { Count = 2, AgeGrade = new AgeGrade(80), Result = new Result { Course = new Course { Distance = new Distance("4 miles") } } }
		};

		//act
		var member = new TeamMember(results);

		//assert
		Assert.Equal(3, member.Runs);
		Assert.Equal(11, member.Miles.Value);
		Assert.Equal(85, member.AgeGrade.Value);
	}
}