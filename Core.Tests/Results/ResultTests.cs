using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Results;

public sealed class ResultTests
{
	[Fact]
	public void CanGetFinishTime()
	{
		//arrange
		var result = new Result
		{
			StartTime = new Date(new DateTime(2024, 04, 15, 09, 25, 00)),
			Duration = new Time(new TimeSpan(1, 2, 3))
		};

		//act
		var finishTime = result.FinishTime;

		//assert
		Assert.Equal(new DateTime(2024, 04, 15, 10, 27, 03), finishTime.Value);
	}

	[Fact]
	public void FinishTimeIsStartTimeWhenDurationIsNull()
	{
		//arrange
		var result = new Result
		{
			StartTime = new Date(new DateTime(2024, 04, 15, 09, 25, 00))
		};

		//act
		var finishTime = result.FinishTime;

		//assert
		Assert.Equal(result.StartTime.Value, finishTime.Value);
	}

	[Fact]
	public void FinishTimeIsNullWhenStartTimeIsNull()
	{
		//arrange
		var result = new Result();

		//act
		var finishTime = result.FinishTime;

		//assert
		Assert.Null(finishTime);
	}

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
	public void CanGetAgeGradeForRoadResult()
	{
		//arrage
		var result = new Result
		{
			StartTime = new Date(new DateTime(2021, 1, 3)),
			Duration = new Time(new TimeSpan(1, 0, 0)),
			Course = new Course { Distance = new Distance("10 miles") },
			Athlete = new Athlete
			{
				Age = 20,
				DateOfBirth = new DateTime(2000, 1, 2)
			}
		};

		var ageGrade = result.AgeGrade;

		//assert
		Assert.Equal(72.1, ageGrade!.Value, 1);
	}

	[Fact]
	public void CanGetAgeGradeForTrackResult()
	{
		//arrage
		var result = new Result
		{
			StartTime = new Date(new DateTime(2021, 1, 3)),
			Duration = new Time(new TimeSpan(0, 4, 30)),
			Course = new Course { Race = new Race { Name = "1mi", Type = "Track" } },
			Athlete = new Athlete
			{
				Age = 20,
				DateOfBirth = new DateTime(2000, 1, 2)
			}
		};

		var ageGrade = result.AgeGrade;

		//assert
		Assert.Equal(82.6, ageGrade!.Value, 1);
	}

	[Fact]
	public void CanGetAgeGradeForMetricFieldResult()
	{
		//arrage
		var result = new Result
		{
			StartTime = new Date(new DateTime(2021, 1, 3)),
			Performance = new Performance("5m"),
			Course = new Course { Race = new Race { Name = "Long Jump", Type = "Field" } },
			Athlete = new Athlete
			{
				Age = 20,
				DateOfBirth = new DateTime(2000, 1, 2)
			}
		};

		var ageGrade = result.AgeGrade;

		//assert
		Assert.Equal(55.9, ageGrade!.Value, 1);
	}

	[Fact]
	public void CanGetAgeGradeForImperialFieldResult()
	{
		//arrage
		var result = new Result
		{
			StartTime = new Date(new DateTime(2021, 1, 3)),
			Performance = new Performance("16'5\""),
			Course = new Course { Race = new Race { Name = "Long Jump", Type = "Field" } },
			Athlete = new Athlete
			{
				Age = 20,
				DateOfBirth = new DateTime(2000, 1, 2)
			}
		};

		var ageGrade = result.AgeGrade;

		//assert
		Assert.Equal(55.9, ageGrade!.Value, 1);
	}

	[Fact]
	public void AgeGradeIsNullWhenDurationIsNullForRoadResult()
	{
		//arrage
		var result = new Result
		{
			StartTime = new Date(new DateTime(2021, 1, 3)),
			Course = new Course { Distance = new Distance("10 miles") },
			Athlete = new Athlete
			{
				Age = 20,
				DateOfBirth = new DateTime(2000, 1, 2)
			}
		};

		var ageGrade = result.AgeGrade;

		//assert
		Assert.Null(ageGrade);
	}

	[Fact]
	public void AgeGradeIsNullWhenDurationIsNullForTrackResult()
	{
		//arrage
		var result = new Result
		{
			StartTime = new Date(new DateTime(2021, 1, 3)),
			Course = new Course { Race = new Race { Name = "1mi", Type = "Track" } },
			Athlete = new Athlete
			{
				Age = 20,
				DateOfBirth = new DateTime(2000, 1, 2)
			}
		};

		var ageGrade = result.AgeGrade;

		//assert
		Assert.Null(ageGrade);
	}

	[Fact]
	public void AgeGradeIsNullWhenDurationIsNullForFieldResult()
	{
		//arrage
		var result = new Result
		{
			StartTime = new Date(new DateTime(2021, 1, 3)),
			Course = new Course { Race = new Race { Name = "Long Jump", Type = "Field" } },
			Athlete = new Athlete
			{
				Age = 20,
				DateOfBirth = new DateTime(2000, 1, 2)
			}
		};

		var ageGrade = result.AgeGrade;

		//assert
		Assert.Null(ageGrade);
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
		var course = new Course();
		var r1 = new Result
		{
			Athlete = CourseData.Athlete1,
			Course = course,
			StartTime = new Date(new DateTime(2022, 4, 7, 16, 42, 0)),
			Duration = new Time(new TimeSpan(1, 2, 3))
		};
		var r2 = new Result
		{
			Athlete = CourseData.Athlete2,
			Course = course,
			StartTime = new Date(new DateTime(2022, 4, 7, 16, 38, 0)),
			Duration = new Time(new TimeSpan(4, 6, 8))
		};
		course.Results = [r1, r2];

		//act
		var isGroupRun = r1.IsGroupRun();

		//assert
		Assert.True(isGroupRun);
	}

	[Fact]
	public void IsNotGroupRunWhenNotWithinThreshold()
	{
		//arrange
		var course = new Course();
		var r1 = new Result
		{
			Athlete = CourseData.Athlete1,
			Course = course,
			StartTime = new Date(new DateTime(2022, 4, 7, 16, 42, 0)),
			Duration = new Time(new TimeSpan(1, 2, 3))
		};
		var r2 = new Result
		{
			Athlete = CourseData.Athlete1,
			Course = course,
			StartTime = new Date(new DateTime(2022, 4, 7, 16, 36, 0)),
			Duration = new Time(new TimeSpan(4, 6, 8))
		};
		course.Results = [r1, r2];

		//act
		var isGroupRun = r1.IsGroupRun();

		//assert
		Assert.False(isGroupRun);
	}

	[Fact]
	public void IsNotGroupRunWhenDifferentCourse()
	{
		//arrange
		var course1 = new Course();
		var r1 = new Result
		{
			Athlete = CourseData.Athlete1,
			Course = course1,
			StartTime = new Date(new DateTime(2022, 4, 7, 16, 42, 0)),
			Duration = new Time(new TimeSpan(1, 2, 3))
		};
		course1.Results = [r1];

		var course2 = new Course();
		var r2 = new Result
		{
			Athlete = CourseData.Athlete1,
			Course = course2,
			StartTime = new Date(new DateTime(2022, 4, 7, 16, 36, 0)),
			Duration = new Time(new TimeSpan(4, 6, 8))
		};
		course2.Results = [r2];

		//act
		var isGroupRun = r1.IsGroupRun();

		//assert
		Assert.False(isGroupRun);
	}

	[Fact]
	public void IsNotGroupRunWhenSameAthlete()
	{
		//arrange
		var course = new Course();
		var r1 = new Result
		{
			Athlete = CourseData.Athlete1,
			Course = course,
			StartTime = new Date(new DateTime(2022, 4, 7, 16, 42, 0)),
			Duration = new Time(new TimeSpan(1, 2, 3))
		};
		var r2 = new Result
		{
			Athlete = CourseData.Athlete1,
			Course = course,
			StartTime = new Date(new DateTime(2022, 4, 7, 16, 38, 0)),
			Duration = new Time(new TimeSpan(4, 6, 8))
		};
		course.Results = [r1, r2];

		//act
		var isGroupRun = r1.IsGroupRun();

		//assert
		Assert.False(isGroupRun);
	}

	[Fact]
	public void HasCommunityStarWhenRunBySameAthleteOnSameDayHasSameStar()
	{
		//arrange
		var course = new Course();

		var r1 = new Result
		{
			Athlete = CourseData.Athlete1,
			Course = course,
			StartTime = new Date(new DateTime(2022, 4, 7, 16, 42, 0))
		};

		var r2 = new Result
		{
			Athlete = CourseData.Athlete1,
			Course = course,
			StartTime = new Date(new DateTime(2022, 4, 7, 16, 38, 0)),
			CommunityStars =
			{
				[StarType.GroupRun] = true
			}
		};
		course.Results = [r1, r2];

		//act
		var hasPoint = r1.HasCommunityStarToday(StarType.GroupRun);

		//assert
		Assert.True(hasPoint);
	}

	[Fact]
	public void DoesNotHaveCommunityStarWhenRunBySameAthleteOnDifferentDayHasSameStar()
	{
		//arrange
		var course = new Course();

		var r1 = new Result
		{
			Athlete = CourseData.Athlete1,
			Course = course,
			StartTime = new Date(new DateTime(2022, 4, 7, 16, 42, 0))
		};

		var r2 = new Result
		{
			Athlete = CourseData.Athlete1,
			Course = course,
			StartTime = new Date(new DateTime(2022, 4, 6, 16, 38, 0)),
			CommunityStars =
			{
				[StarType.GroupRun] = true
			}
		};
		course.Results = [r1, r2];

		//act
		var hasPoint = r1.HasCommunityStarToday(StarType.GroupRun);

		//assert
		Assert.False(hasPoint);
	}

	[Fact]
	public void DoesNotHaveCommunityStarWhenRunByDifferentAthleteOnSameDayHasSameStar()
	{
		//arrange
		var course = new Course();

		var r1 = new Result
		{
			Athlete = CourseData.Athlete1,
			Course = course,
			StartTime = new Date(new DateTime(2022, 4, 7, 16, 42, 0))
		};

		var r2 = new Result
		{
			Athlete = CourseData.Athlete2,
			Course = course,
			StartTime = new Date(new DateTime(2022, 4, 7, 16, 38, 0)),
			CommunityStars =
			{
				[StarType.GroupRun] = true
			}
		};
		course.Results = [r1, r2];

		//act
		var hasPoint = r1.HasCommunityStarToday(StarType.GroupRun);

		//assert
		Assert.False(hasPoint);
	}

	[Fact]
	public void DoesNotHaveCommunityStarWhenRunBySameAthleteOnSameDayHasDifferentStar()
	{
		//arrange
		var course = new Course();

		var r1 = new Result
		{
			Athlete = CourseData.Athlete1,
			Course = course,
			StartTime = new Date(new DateTime(2022, 4, 7, 16, 42, 0))
		};

		var r2 = new Result
		{
			Athlete = CourseData.Athlete2,
			Course = course,
			StartTime = new Date(new DateTime(2022, 4, 7, 16, 38, 0)),
			CommunityStars =
			{
				[StarType.GroupRun] = true
			}
		};
		course.Results = [r1, r2];

		//act
		var hasPoint = r1.HasCommunityStarToday(StarType.Story);

		//assert
		Assert.False(hasPoint);
	}
}