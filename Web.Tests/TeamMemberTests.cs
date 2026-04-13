using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Web.ViewModels;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests;

public sealed class TeamMemberTests
{
	[Fact]
	public void CanGetTotalsForAthlete()
	{
		//arrange
		var results = new Ranked<Time, Result>[]
		{
			new() { Count = 1, AgeGrade = new AgeGrade(90), Result = new Result { Course = new Course { Distance = 3, Units = "mi" } } },
			new() { Count = 2, AgeGrade = new AgeGrade(80), Result = new Result { Course = new Course { Distance = 4, Units = "mi"  } } }
		};

		//act
		var member = new TeamMember(results);

		//assert
		Assert.Equal(3, member.Runs);
		Assert.Equal(11, member.Miles.Value);
		Assert.Equal(85, member.AgeGrade.Value);
	}
}