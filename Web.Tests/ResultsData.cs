using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.Tests;

public static class ResultsData
{
	public static readonly Athlete Athlete1 = new() { ID = Guid.NewGuid(), Name="A1", Category = 'M', DateOfBirth = DateOnly.Parse("1/1/2000") };
	public static readonly Athlete Athlete2 = new() { ID = Guid.NewGuid(), Name="A2", Category = 'F', DateOfBirth = DateOnly.Parse("1/1/2000") };
	public static readonly Athlete Athlete3 = new() { ID = Guid.NewGuid(), Name="A3", Category = 'M', DateOfBirth = DateOnly.Parse("1/1/1990") };
	public static readonly Athlete Athlete4 = new() { ID = Guid.NewGuid(), Name="A4", Category = 'F', DateOfBirth = DateOnly.Parse("1/1/1990") };
	public static readonly Athlete Private = new() { ID = Guid.NewGuid(), Name="AP", Category = 'F', DateOfBirth = DateOnly.Parse("1/1/1980"), IsPrivate = true };

	private static readonly Race Race = new() { Name = "10K", Type = "Road" };
	public static readonly Course Course = new() { Race = Race, Distance = 10, Units = "km" };

	public static Result[] Results =>
	[
		new() { Course = Course, Athlete = Athlete1, StartTime = DateTime.Parse("2/1/2020"), Duration = TimeSpan.Parse("2:34:56.7") },
		new() { Course = Course, Athlete = Athlete1, StartTime = DateTime.Parse("2/3/2020"), Duration = TimeSpan.Parse("1:20:00.0") },
		new() { Course = Course, Athlete = Athlete2, StartTime = DateTime.Parse("2/7/2020"), Duration = TimeSpan.Parse("0:54:32.1") },
		new() { Course = Course, Athlete = Athlete3, StartTime = DateTime.Parse("2/5/2020"), Duration = TimeSpan.Parse("1:02:03.4") },
		new() { Course = Course, Athlete = Athlete3, StartTime = DateTime.Parse("2/2/2020"), Duration = TimeSpan.Parse("1:00:00.0") },
		new() { Course = Course, Athlete = Athlete4, StartTime = DateTime.Parse("2/6/2020"), Duration = TimeSpan.Parse("2:03:04.5") },
		new() { Course = Course, Athlete = Athlete4, StartTime = DateTime.Parse("2/8/2020"), Duration = TimeSpan.Parse("2:22:22.2") },
		new() { Course = Course, Athlete = Athlete4, StartTime = DateTime.Parse("2/4/2020"), Duration = TimeSpan.Parse("2:00:00.0") }
	];

	private static readonly Course SimilarCourse = new() { Distance = 400, Units = "m" };
	public static Result[] SimilarResults =>
	[
		new() { Course = SimilarCourse, Athlete = Athlete1, StartTime =DateTime.Parse("2/1/2020"), Duration = TimeSpan.FromSeconds(100) },
		new() { Course = SimilarCourse, Athlete = Athlete2, StartTime =DateTime.Parse("2/3/2020"), Duration = TimeSpan.FromSeconds(105) },
		new() { Course = SimilarCourse, Athlete = Athlete3, StartTime =DateTime.Parse("2/7/2020"), Duration = TimeSpan.FromSeconds(110) },
		new() { Course = SimilarCourse, Athlete = Athlete4, StartTime =DateTime.Parse("2/5/2020"), Duration = TimeSpan.FromSeconds(96) }
	];
}