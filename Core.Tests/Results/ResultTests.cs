using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Results;

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

	[Fact]
	public void IsGroupRunWhenSomeoneElseStartsWithinLimit()
	{
		//arrange
		var results = new List<Result>();
		var course = new Course
		{
			Results = results
		};

		var r1 = new Result
		{
			Athlete = CourseData.Athlete1,
			Course = course,
			StartTime = new Date(new DateTime(2022, 4, 7, 4, 42, 0)),
			Duration = new Time(new TimeSpan(1, 2, 3))
		};
		var r2 = new Result
		{
			Athlete = CourseData.Athlete2,
			Course = course,
			StartTime = new Date(new DateTime(2022, 4, 7, 4, 38, 0)),
			Duration = new Time(new TimeSpan(4, 6, 8))
		};
		results.AddRange(new[] { r1, r2 });

		//act
		var isGroupRun = r1.IsGroupRun();

		//assert
		Assert.True(isGroupRun);
	}

	[Fact]
	public void IsNotGroupRunWhenNotWithinThreshold()
	{
		//arrange
		var results = new List<Result>();
		var course = new Course
		{
			Results = results
		};

		var r1 = new Result
		{
			Athlete = CourseData.Athlete1,
			Course = course,
			StartTime = new Date(new DateTime(2022, 4, 7, 4, 42, 0)),
			Duration = new Time(new TimeSpan(1, 2, 3))
		};
		var r2 = new Result
		{
			Athlete = CourseData.Athlete1,
			Course = course,
			StartTime = new Date(new DateTime(2022, 4, 7, 4, 36, 0)),
			Duration = new Time(new TimeSpan(4, 6, 8))
		};
		results.AddRange(new[] { r1, r2 });

		//act
		var isGroupRun = r1.IsGroupRun();

		//assert
		Assert.False(isGroupRun);
	}

	[Fact]
	public void IsNotGroupRunWhenDifferentCourse()
	{
		//arrange
		var results1 = new List<Result>();
		var course1 = new Course
		{
			Results = results1
		};
		var r1 = new Result
		{
			Athlete = CourseData.Athlete1,
			Course = course1,
			StartTime = new Date(new DateTime(2022, 4, 7, 4, 42, 0)),
			Duration = new Time(new TimeSpan(1, 2, 3))
		};
		results1.AddRange(new[] { r1 });

		var results2 = new List<Result>();
		var course2 = new Course
		{
			Results = results2
		};
		var r2 = new Result
		{
			Athlete = CourseData.Athlete1,
			Course = course2,
			StartTime = new Date(new DateTime(2022, 4, 7, 4, 36, 0)),
			Duration = new Time(new TimeSpan(4, 6, 8))
		};
		results2.AddRange(new[] { r2 });

		//act
		var isGroupRun = r1.IsGroupRun();

		//assert
		Assert.False(isGroupRun);
	}

	[Fact]
	public void IsNotGroupRunWhenSameAthlete()
	{
		//arrange
		var results = new List<Result>();
		var course = new Course
		{
			Results = results
		};

		var r1 = new Result
		{
			Athlete = CourseData.Athlete1,
			Course = course,
			StartTime = new Date(new DateTime(2022, 4, 7, 4, 42, 0)),
			Duration = new Time(new TimeSpan(1, 2, 3))
		};
		var r2 = new Result
		{
			Athlete = CourseData.Athlete1,
			Course = course,
			StartTime = new Date(new DateTime(2022, 4, 7, 4, 38, 0)),
			Duration = new Time(new TimeSpan(4, 6, 8))
		};
		results.AddRange(new[] { r1, r2 });

		//act
		var isGroupRun = r1.IsGroupRun();

		//assert
		Assert.False(isGroupRun);
	}
}