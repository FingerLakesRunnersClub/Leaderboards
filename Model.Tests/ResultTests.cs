using Xunit;

namespace FLRC.Leaderboards.Model.Tests;

public sealed class ResultTests
{
	[Fact]
	public void CanGetAthleteAgeOnDayOfRun()
	{
		//arrange
		var result = new Result
		{
			StartTime = new DateTime(2021, 1, 3),
			Athlete = new Athlete
			{
				DateOfBirth = new DateOnly(2000, 1, 2)
			}
		};

		//act
		var age = result.AthleteAge;

		//assert
		Assert.Equal(21, age!.Value);
	}
}