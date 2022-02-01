namespace FLRC.Leaderboards.Core.Tests;

public static class CourseData
{
	public static readonly Athlete Athlete1 = new() { ID = 123, Category = Category.M, Age = 20, DateOfBirth = DateTime.Parse("1/1/2000") };
	public static readonly Athlete Athlete2 = new() { ID = 234, Category = Category.F, Age = 20, DateOfBirth = DateTime.Parse("1/1/2000") };
	public static readonly Athlete Athlete3 = new() { ID = 345, Category = Category.M, Age = 30, DateOfBirth = DateTime.Parse("1/1/1990") };
	public static readonly Athlete Athlete4 = new() { ID = 456, Category = Category.F, Age = 30, DateOfBirth = DateTime.Parse("1/1/1990") };

	public static readonly Course Course = new() { Meters = 10000 };

	public static IEnumerable<Result> Results => new List<Result>
		{
			new() {Course = Course, Athlete = Athlete1, StartTime = new Date(DateTime.Parse("1/1/2020")), Duration = new Time(TimeSpan.Parse("2:34:56.7"))},
			new() {Course = Course, Athlete = Athlete1, StartTime = new Date(DateTime.Parse("1/3/2020")), Duration = new Time(TimeSpan.Parse("1:20:00.0"))},
			new() {Course = Course, Athlete = Athlete2, StartTime = new Date(DateTime.Parse("1/7/2020")), Duration = new Time(TimeSpan.Parse("0:54:32.1"))},
			new() {Course = Course, Athlete = Athlete3, StartTime = new Date(DateTime.Parse("1/5/2020")), Duration = new Time(TimeSpan.Parse("1:02:03.4"))},
			new() {Course = Course, Athlete = Athlete3, StartTime = new Date(DateTime.Parse("1/2/2020")), Duration = new Time(TimeSpan.Parse("1:00:00.0"))},
			new() {Course = Course, Athlete = Athlete4, StartTime = new Date(DateTime.Parse("1/6/2020")), Duration = new Time(TimeSpan.Parse("2:03:04.5"))},
			new() {Course = Course, Athlete = Athlete4, StartTime = new Date(DateTime.Parse("1/8/2020")), Duration = new Time(TimeSpan.Parse("2:22:22.2"))},
			new() {Course = Course, Athlete = Athlete4, StartTime = new Date(DateTime.Parse("1/4/2020")), Duration = new Time(TimeSpan.Parse("2:00:00.0"))}
		};

	public static IEnumerable<Result> SimilarResults => new List<Result>
		{
			new() {Course = Course, Athlete = Athlete1, Duration = new Time(TimeSpan.FromSeconds(100))},
			new() {Course = Course, Athlete = Athlete2, Duration = new Time(TimeSpan.FromSeconds(105))},
			new() {Course = Course, Athlete = Athlete3, Duration = new Time(TimeSpan.FromSeconds(110))},
			new() {Course = Course, Athlete = Athlete4, Duration = new Time(TimeSpan.FromSeconds(96))}
		};
}