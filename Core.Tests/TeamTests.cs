using Xunit;

namespace FLRC.Leaderboards.Core.Tests;

public class TeamTests
{
	[Theory]
	[InlineData(2, "1–29")]
	[InlineData(3, "30–39")]
	[InlineData(4, "40–49")]
	[InlineData(5, "50–59")]
	[InlineData(6, "60+")]
	public void CanGetTeamDisplayNameFromAge(byte id, string expected)
	{
		//arrange
		var team = new Team(id);

		//act
		var teamName = team.Display;

		//assert
		Assert.Equal(expected, teamName);
	}
}