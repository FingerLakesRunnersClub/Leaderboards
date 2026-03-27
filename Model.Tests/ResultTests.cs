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

	[Fact]
	public void CanGetFinishTime()
	{
		//arrange
		var result = new Result
		{
			StartTime = new DateTime(2024, 04, 15, 09, 25, 00),
			Duration = new TimeSpan(1, 2, 3)
		};

		//act
		var finishTime = result.FinishTime;

		//assert
		Assert.Equal(new DateTime(2024, 04, 15, 10, 27, 03), finishTime);
	}


}