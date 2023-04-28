using FLRC.Leaderboards.Core.Athletes;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Athletes;

public sealed class AthleteTests
{
	[Theory]
	[InlineData(2000, 01, 01, 21)]
	[InlineData(2000, 01, 02, 20)]
	[InlineData(1950, 01, 01, 71)]
	[InlineData(1950, 01, 02, 70)]
	public void CanGetAgeAsOfACertainDate(ushort year, byte month, byte day, byte expected)
	{
		//arrange
		var athlete = new Athlete { DateOfBirth = new DateTime(year, month, day) };

		var asOf = new DateTime(2021, 1, 1);

		//act
		var age = athlete.AgeAsOf(asOf);

		//assert
		Assert.Equal(expected, age);
	}

	[Theory]
	[InlineData(1, 2)]
	[InlineData(29, 2)]
	[InlineData(36, 3)]
	[InlineData(45, 4)]
	[InlineData(58, 5)]
	[InlineData(60, 6)]
	[InlineData(80, 6)]
	public void CanGetTeamForAge(byte age, byte expected)
	{
		//arrange
		var athlete = new Athlete { Age = age };

		//act
		var team = athlete.Team;

		//assert
		Assert.Equal(expected, team.Value);
	}
}