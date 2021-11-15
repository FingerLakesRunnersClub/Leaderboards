using System;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests;

public class ResultTests
{
	[Fact]
	public void CanGetAgeOnDayOfRun()
	{
		//arrange
		var result = new Result
		{
			StartTime = new Date(new DateTime(2021, 1, 3)),
			Athlete = new Athlete
			{
				Age = 20,
				DateOfBirth = new DateTime(2000, 1, 2)
			}
		};

		//act
		var ageOnDayOfRun = result.AgeOnDayOfRun;

		//assert
		Assert.Equal(21, ageOnDayOfRun);
	}

	[Fact]
	public void CanCompareResultsByDuration()
	{
		//arrange
		var result1 = new Result { Duration = new Time(new TimeSpan(2, 3, 4)) };

		//act
		var result2 = new Result { Duration = new Time(new TimeSpan(1, 2, 3)) };

		//assert
		Assert.Equal(1, result1.CompareTo(result2));
		Assert.Equal(-1, result2.CompareTo(result1));
	}

	[Fact]
	public void CanGetTimeBehindOtherResult()
	{
		//arrange
		var r1 = new Result { Duration = new Time(new TimeSpan(1, 2, 3)) };
		var r2 = new Result { Duration = new Time(new TimeSpan(4, 6, 8)) };

		//act
		var behind = r2.Behind(r1);

		//assert
		Assert.Equal(new Time(new TimeSpan(3, 4, 5)), behind);
	}
}
