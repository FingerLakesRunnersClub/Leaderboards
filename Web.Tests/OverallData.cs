using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.Tests;

public static class OverallData
{
	public static readonly Athlete Athlete1 = new() { ID = Guid.NewGuid(), Name = "A1", DateOfBirth = new DateOnly(2000, 1, 1), Category = 'M' };
	public static readonly Athlete Athlete2 = new() { ID = Guid.NewGuid(), Name = "A2", DateOfBirth = new DateOnly(1990, 1, 1), Category = 'M' };
	public static readonly Athlete Athlete3 = new() { ID = Guid.NewGuid(), Name = "A3", DateOfBirth = new DateOnly(2000, 1, 1), Category = 'F' };
	public static readonly Athlete Athlete4 = new() { ID = Guid.NewGuid(), Name = "A4", DateOfBirth = new DateOnly(1990, 1, 1), Category = 'F' };
	public static readonly Athlete Private = new() { ID = Guid.NewGuid(), Name = "A5", DateOfBirth = new DateOnly(1980, 1, 1), Category = 'F', IsPrivate = true };

	private static readonly Race Race2 = new() { ID = Guid.NewGuid(), Name = "Test", Type = "Road" };
	private static readonly Course Course2 = new() { ID = Guid.NewGuid(), Distance = 10, Units = "mi", Race = Race2 };

	private static Result[] Results
		=>
		[
			new() { ID = Guid.NewGuid(), Course = Course2, Athlete = Athlete1, StartTime = DateTime.Parse("1/2/2020"), Duration = new TimeSpan(1, 2, 3) },
			new() { ID = Guid.NewGuid(), Course = Course2, Athlete = Athlete1, StartTime = DateTime.Parse("1/5/2020"), Duration = new TimeSpan(1, 23, 45) },
			new() { ID = Guid.NewGuid(), Course = Course2, Athlete = Athlete2, StartTime = DateTime.Parse("1/6/2020"), Duration = new TimeSpan(2, 3, 4) },
			new() { ID = Guid.NewGuid(), Course = Course2, Athlete = Athlete2, StartTime = DateTime.Parse("1/3/2020"), Duration = new TimeSpan(2, 34, 56) },
			new() { ID = Guid.NewGuid(), Course = Course2, Athlete = Athlete2, StartTime = DateTime.Parse("1/7/2020"), Duration = new TimeSpan(2, 22, 22) },
			new() { ID = Guid.NewGuid(), Course = Course2, Athlete = Athlete3, StartTime = DateTime.Parse("1/4/2020"), Duration = new TimeSpan(3, 2, 1) },
			new() { ID = Guid.NewGuid(), Course = Course2, Athlete = Athlete3, StartTime = DateTime.Parse("1/9/2020"), Duration = new TimeSpan(3, 21, 0) },
			new() { ID = Guid.NewGuid(), Course = Course2, Athlete = Athlete4, StartTime = DateTime.Parse("1/8/2020"), Duration = new TimeSpan(4, 3, 2) },
			new() { ID = Guid.NewGuid(), Course = Course2, Athlete = Athlete4, StartTime = DateTime.Parse("1/10/2020"), Duration = new TimeSpan(4, 32, 10) },
			new() { ID = Guid.NewGuid(), Course = Course2, Athlete = Athlete4, StartTime = DateTime.Parse("1/1/2020"), Duration = new TimeSpan(4, 4, 4) }
		];

	private static readonly Course Course1 = new() { ID = Guid.NewGuid(), Distance = 10, Units = "mi", Results = Results };
	private static readonly Race Race1 = new() { ID = Guid.NewGuid(), Name = "Test", Type = "Road", Courses = [Course1] };

	private static readonly Course Course3 = new() { ID = Guid.NewGuid(), Distance = 10, Units = "mi", Results = Results };
	private static readonly Race Race3 = new() { ID = Guid.NewGuid(), Name = "Test", Type = "Road", Courses = [Course3] };

	private static readonly Challenge OfficialChallenge = new() { IsOfficial = true, IsPrimary = true, Courses = [Course1] };
	public static readonly Iteration Iteration = new() { ID = Guid.NewGuid(), Races = [Race1, Race3], Challenges = [OfficialChallenge] };
}