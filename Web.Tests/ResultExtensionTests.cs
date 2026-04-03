using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Model;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests;

public sealed class ResultExtensionTests
{
	[Fact]
	public void ResultIsValidWithRealisticAgeGrade()
	{
		//arrange
		var result = new Result
		{
			StartTime = new DateTime(2020, 1, 1, 12, 0, 0),
			Duration = new TimeSpan(1, 2, 3),
			Course = new Course { Distance = 10, Units = "km", Race = new Race { Type = "Road" } },
			Athlete = new Athlete { DateOfBirth = new DateOnly(2000, 1, 1) }
		};

		//act
		var isValid = result.IsValid();

		//assert
		Assert.True(isValid);
	}

	[Fact]
	public void ResultIsNotValidWithUnrealisticAgeGrade()
	{
		//arrange
		var result = new Result
		{
			Duration = new TimeSpan(1, 2, 3),
			Course = new Course { Distance = 40, Units = "km", Race = new Race { Type = "Road" } },
			Athlete = new Athlete { DateOfBirth = new DateOnly(2020, 1, 1) }
		};

		//act
		var isValid = result.IsValid();

		//assert
		Assert.False(isValid);
	}

	[Fact]
	public void ResultIsNotValidWithoutAthleteAge()
	{
		//arrange
		var result = new Result
		{
			Duration = new TimeSpan(1, 2, 3),
			Course = new Course { Distance = 40, Units = "km", Race = new Race { Type = "Road" } },
			Athlete = new Athlete()
		};

		//act
		var isValid = result.IsValid();

		//assert
		Assert.False(isValid);
	}

	[Fact]
	public void CanGetTimeBehindOtherResult()
	{
		//arrange
		var r1 = new Result { Duration = new TimeSpan(1, 2, 3) };
		var r2 = new Result { Duration = new TimeSpan(4, 6, 8) };

		//act
		var behind = r2.Behind(r1);

		//assert
		Assert.Equal(new Time(new TimeSpan(3, 4, 5)), behind);
	}
}