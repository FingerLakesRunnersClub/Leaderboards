using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Tests;

public static class CourseData
{
	public static readonly Athlete Athlete1 = new() { ID = 123, Category = Category.M, Age = 20, DateOfBirth = DateTime.Parse("1/1/2000") };
	public static readonly Athlete Athlete2 = new() { ID = 234, Category = Category.F, Age = 20, DateOfBirth = DateTime.Parse("1/1/2000") };
	public static readonly Athlete Athlete3 = new() { ID = 345, Category = Category.M, Age = 30, DateOfBirth = DateTime.Parse("1/1/1990") };
	public static readonly Athlete Athlete4 = new() { ID = 456, Category = Category.F, Age = 30, DateOfBirth = DateTime.Parse("1/1/1990") };
	public static readonly Athlete Private = new() { ID = 567, Category = Category.F, Age = 40, DateOfBirth = DateTime.Parse("1/1/1980"), Private = true };

	public static readonly Course Course = new() { Distance = new Distance("10K") };

	public static Result[] Results =>
	[
		new Result { Course = Course, Athlete = Athlete1, StartTime = new Date(DateTime.Parse("2/1/2020")), Duration = new Time(TimeSpan.Parse("2:34:56.7")) },
		new Result { Course = Course, Athlete = Athlete1, StartTime = new Date(DateTime.Parse("2/3/2020")), Duration = new Time(TimeSpan.Parse("1:20:00.0")) },
		new Result { Course = Course, Athlete = Athlete2, StartTime = new Date(DateTime.Parse("2/7/2020")), Duration = new Time(TimeSpan.Parse("0:54:32.1")) },
		new Result { Course = Course, Athlete = Athlete3, StartTime = new Date(DateTime.Parse("2/5/2020")), Duration = new Time(TimeSpan.Parse("1:02:03.4")) },
		new Result { Course = Course, Athlete = Athlete3, StartTime = new Date(DateTime.Parse("2/2/2020")), Duration = new Time(TimeSpan.Parse("1:00:00.0")) },
		new Result { Course = Course, Athlete = Athlete4, StartTime = new Date(DateTime.Parse("2/6/2020")), Duration = new Time(TimeSpan.Parse("2:03:04.5")) },
		new Result { Course = Course, Athlete = Athlete4, StartTime = new Date(DateTime.Parse("2/8/2020")), Duration = new Time(TimeSpan.Parse("2:22:22.2")) },
		new Result { Course = Course, Athlete = Athlete4, StartTime = new Date(DateTime.Parse("2/4/2020")), Duration = new Time(TimeSpan.Parse("2:00:00.0")) }
	];

	private static readonly Course SimilarCourse = new() { Distance = new Distance("400m") };
	public static Result[] SimilarResults =>
	[
		new Result { Course = SimilarCourse, Athlete = Athlete1, StartTime = new Date(DateTime.Parse("2/1/2020")), Duration = new Time(TimeSpan.FromSeconds(100)) },
		new Result { Course = SimilarCourse, Athlete = Athlete2, StartTime = new Date(DateTime.Parse("2/3/2020")), Duration = new Time(TimeSpan.FromSeconds(105)) },
		new Result { Course = SimilarCourse, Athlete = Athlete3, StartTime = new Date(DateTime.Parse("2/7/2020")), Duration = new Time(TimeSpan.FromSeconds(110)) },
		new Result { Course = SimilarCourse, Athlete = Athlete4, StartTime = new Date(DateTime.Parse("2/5/2020")), Duration = new Time(TimeSpan.FromSeconds(96)) }
	];
}