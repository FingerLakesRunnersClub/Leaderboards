using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Races;

public class CoursesExtensionsTests
{
	[Fact]
	public void CanGetDistinctMonthsOfResults()
	{
		//arrange
		var courses = new[]
		{
			new Course
			{
				Results = new[]
				{
					new Result { Athlete = CourseData.Athlete3, StartTime = new Date(new DateTime(2019, 12, 31)), Duration = new Time(new TimeSpan(1, 2, 3)) },
					new Result { Athlete = CourseData.Athlete3, StartTime = new Date(new DateTime(2020, 1, 2)), Duration = new Time(new TimeSpan(2, 3, 4)) },
				}
			},
			new Course
			{
				Results = new[]
				{
					new Result { Athlete = CourseData.Athlete3, StartTime = new Date(new DateTime(2020, 1, 2)), Duration = new Time(new TimeSpan(2, 3, 4)) },
					new Result { Athlete = CourseData.Athlete3, StartTime = new Date(new DateTime(2020, 2, 2)), Duration = new Time(new TimeSpan(2, 3, 4)) },
				}
			}
		};

		//act
		var months = courses.DistinctMonths();

		//assert
		Assert.Equal(3, months.Count);
		Assert.Equal(new DateOnly(2019, 12, 1), months.First());
		Assert.Equal(new DateOnly(2020, 1, 1), months.Skip(1).First());
		Assert.Equal(new DateOnly(2020, 2, 1), months.Skip(2).First());
	}
}