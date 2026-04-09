using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.Tests;

public static class UltraChallengeData
{
	private static readonly Guid Road1ID = Guid.NewGuid();
	private static readonly Guid Road2ID = Guid.NewGuid();
	private static readonly Guid Trail1ID = Guid.NewGuid();
	private static readonly Guid Trail2ID = Guid.NewGuid();

	private static readonly Course Course = new() { Race = new Race { ID = Guid.NewGuid(), Type = "Road" } };

	public static readonly Course Road1 = new()
	{
		ID = Road1ID,
		Distance = 25,
		Units = "km",
		Race = new Race { ID = Guid.NewGuid(), Type = "Road" },
		Results =
		[
			new Result { ID = Guid.NewGuid(), CourseID = Road1ID, Course = Course, Athlete = ResultsData.Athlete1, StartTime = DateTime.Parse("5/20/2024 6am"), Duration = TimeSpan.FromHours(2) },
			new Result { ID = Guid.NewGuid(), CourseID = Road1ID, Course = Course, Athlete = ResultsData.Athlete1, StartTime = DateTime.Parse("5/21/2024 6am"), Duration = TimeSpan.FromHours(2.5) },
			new Result { ID = Guid.NewGuid(), CourseID = Road1ID, Course = Course, Athlete = ResultsData.Athlete2, StartTime = DateTime.Parse("5/20/2024 7am"), Duration = TimeSpan.FromHours(3) },
			new Result { ID = Guid.NewGuid(), CourseID = Road1ID, Course = Course, Athlete = ResultsData.Athlete2, StartTime = DateTime.Parse("5/21/2024 5am"), Duration = TimeSpan.FromHours(3.5) }
		]
	};

	public static readonly Course Road2 = new()
	{
		ID = Road2ID,
		Distance = 25,
		Units = "km",
		Race = new Race { ID = Guid.NewGuid(), Type = "Road" },
		Results =
		[
			new Result { ID = Guid.NewGuid(), CourseID = Road2ID, Course = Course, Athlete = ResultsData.Athlete1, StartTime = DateTime.Parse("5/20/2024 11am"), Duration = TimeSpan.FromHours(2) },
			new Result { ID = Guid.NewGuid(), CourseID = Road2ID, Course = Course, Athlete = ResultsData.Athlete1, StartTime = DateTime.Parse("5/21/2024 9am"), Duration = TimeSpan.FromHours(2.5) },
			new Result { ID = Guid.NewGuid(), CourseID = Road2ID, Course = Course, Athlete = ResultsData.Athlete2, StartTime = DateTime.Parse("5/20/2024 11am"), Duration = TimeSpan.FromHours(3) },
			new Result { ID = Guid.NewGuid(), CourseID = Road2ID, Course = Course, Athlete = ResultsData.Athlete2, StartTime = DateTime.Parse("5/21/2024 11am"), Duration = TimeSpan.FromHours(3.5) }
		]
	};

	public static readonly Course Trail1 = new()
	{
		ID = Trail1ID,
		Distance = 25,
		Units = "km",
		Race = new Race { ID = Guid.NewGuid(), Type = "Trail" },
		Results =
		[
			new Result { ID = Guid.NewGuid(), CourseID = Trail1ID, Course = Course, Athlete = ResultsData.Athlete1, StartTime = DateTime.Parse("5/20/2024 4pm"), Duration = TimeSpan.FromHours(2) },
			new Result { ID = Guid.NewGuid(), CourseID = Trail1ID, Course = Course, Athlete = ResultsData.Athlete1, StartTime = DateTime.Parse("5/21/2024 12pm"), Duration = TimeSpan.FromHours(2.5) },
			new Result { ID = Guid.NewGuid(), CourseID = Trail1ID, Course = Course, Athlete = ResultsData.Athlete2, StartTime = DateTime.Parse("5/20/2024 3pm"), Duration = TimeSpan.FromHours(3) },
			new Result { ID = Guid.NewGuid(), CourseID = Trail1ID, Course = Course, Athlete = ResultsData.Athlete2, StartTime = DateTime.Parse("5/21/2024 5pm"), Duration = TimeSpan.FromHours(3.5) },
			new Result { ID = Guid.NewGuid(), CourseID = Trail1ID, Course = Course, Athlete = ResultsData.Athlete2, StartTime = DateTime.Parse("5/22/2024 8am"), Duration = TimeSpan.FromHours(4) }
		]
	};

	public static readonly Course Trail2 = new()
	{
		ID = Trail2ID,
		Distance = 25,
		Units = "km",
		Race = new Race { ID = Guid.NewGuid(), Type = "Trail" },
		Results =
		[
			new Result { ID = Guid.NewGuid(), CourseID = Trail2ID, Course = Course, Athlete = ResultsData.Athlete1, StartTime = DateTime.Parse("5/20/2024 9pm"), Duration = TimeSpan.FromHours(2) },
			new Result { ID = Guid.NewGuid(), CourseID = Trail2ID, Course = Course, Athlete = ResultsData.Athlete1, StartTime = DateTime.Parse("5/21/2024 3pm"), Duration = TimeSpan.FromHours(2.5) },
			new Result { ID = Guid.NewGuid(), CourseID = Trail2ID, Course = Course, Athlete = ResultsData.Athlete2, StartTime = DateTime.Parse("5/20/2024 7pm"), Duration = TimeSpan.FromHours(3) },
			new Result { ID = Guid.NewGuid(), CourseID = Trail2ID, Course = Course, Athlete = ResultsData.Athlete2, StartTime = DateTime.Parse("5/21/2024 11pm"), Duration = TimeSpan.FromHours(3.5) }
		]
	};

	public static readonly Challenge OneHundredK = new() { ID = Guid.NewGuid(), Name = "Test 1", IsOfficial = true, IsPrimary = false, TimeLimit = TimeSpan.FromHours(20), Courses = [Road1, Road2, Trail1, Trail2] };
	public static readonly Challenge Road = new() { ID = Guid.NewGuid(), Name = "Test 2", IsOfficial = true, IsPrimary = false, TimeLimit = TimeSpan.FromHours(10), Courses = [Road1, Road2] };
	public static readonly Challenge Trail = new() { ID = Guid.NewGuid(), Name = "Test 3", IsOfficial = true, IsPrimary = false, TimeLimit = TimeSpan.FromHours(10), Courses = [Trail1, Trail2] };
	public static readonly Iteration Iteration = new() { ID = Guid.NewGuid(), StartDate = new DateOnly(2024, 1, 1), EndDate = new DateOnly(2024, 12, 31), Challenges = [OneHundredK, Road, Trail] };
}