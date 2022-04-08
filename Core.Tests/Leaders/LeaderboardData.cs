using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Tests.Leaders;

public static class LeaderboardData
{
	public static readonly Athlete Athlete1 = new() { ID = 123, Name = "A1", Age = 20, DateOfBirth = new DateTime(2000, 1, 1), Category = Category.M };
	public static readonly Athlete Athlete2 = new() { ID = 234, Name = "A2", Age = 30, DateOfBirth = new DateTime(1990, 1, 1), Category = Category.M };
	public static readonly Athlete Athlete3 = new() { ID = 345, Name = "A3", Age = 20, DateOfBirth = new DateTime(2000, 1, 1), Category = Category.F };
	public static readonly Athlete Athlete4 = new() { ID = 456, Name = "A4", Age = 30, DateOfBirth = new DateTime(1990, 1, 1), Category = Category.F };

	public static readonly IReadOnlyCollection<Course> Courses = new List<Course>
	{
		new() { Race = new Race { Name = "Test" }, Distance = new Distance("10 miles"), Results = Results }
	};

	private static Course Course => new() { Distance = new Distance("10 miles") };

	private static IReadOnlyCollection<Result> Results
		=> new List<Result>
		{
			new() { Course = Course, Athlete = Athlete1, StartTime = new Date(DateTime.Parse("1/2/2020")), Duration = new Time(new TimeSpan(1, 2, 3)), CommunityPoints = { [PointType.Narrative] = true, [PointType.LocalBusiness] = true }},
			new() { Course = Course, Athlete = Athlete1, StartTime = new Date(DateTime.Parse("1/5/2020")), Duration = new Time(new TimeSpan(1, 23, 45)), CommunityPoints = { [PointType.Narrative] = true, [PointType.LocalBusiness] = true }},
			new() { Course = Course, Athlete = Athlete2, StartTime = new Date(DateTime.Parse("1/6/2020")), Duration = new Time(new TimeSpan(2, 3, 4)), CommunityPoints = { [PointType.Narrative] = true }},
			new() { Course = Course, Athlete = Athlete2, StartTime = new Date(DateTime.Parse("1/3/2020")), Duration = new Time(new TimeSpan(2, 34, 56)) },
			new() { Course = Course, Athlete = Athlete2, StartTime = new Date(DateTime.Parse("1/7/2020")), Duration = new Time(new TimeSpan(2, 22, 22)) },
			new() { Course = Course, Athlete = Athlete3, StartTime = new Date(DateTime.Parse("1/4/2020")), Duration = new Time(new TimeSpan(3, 2, 1)), CommunityPoints = { [PointType.Narrative] = true, [PointType.LocalBusiness] = true }},
			new() { Course = Course, Athlete = Athlete3, StartTime = new Date(DateTime.Parse("1/9/2020")), Duration = new Time(new TimeSpan(3, 21, 0)), CommunityPoints = { [PointType.Narrative] = true }},
			new() { Course = Course, Athlete = Athlete4, StartTime = new Date(DateTime.Parse("1/8/2020")), Duration = new Time(new TimeSpan(4, 3, 2)), CommunityPoints = { [PointType.Narrative] = true }},
			new() { Course = Course, Athlete = Athlete4, StartTime = new Date(DateTime.Parse("1/10/2020")), Duration = new Time(new TimeSpan(4, 32, 10)), CommunityPoints = { [PointType.Narrative] = true }},
			new() { Course = Course, Athlete = Athlete4, StartTime = new Date(DateTime.Parse("1/1/2020")), Duration = new Time(new TimeSpan(4, 4, 4)) }
		};
}