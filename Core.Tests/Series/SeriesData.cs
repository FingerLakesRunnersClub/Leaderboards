using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Tests.Series;

public static class SeriesData
{
	public static readonly Course Road1 = new() { ID = 123, Distance = new Distance("25K"), Race = new Race { ID = 123 } };
	public static readonly Course Road2 = new() { ID = 234, Distance = new Distance("25K"), Race = new Race { ID = 234 } };
	public static readonly Course Trail1 = new() { ID = 345, Distance = new Distance("25K"), Race = new Race { ID = 345 } };
	public static readonly Course Trail2 = new() { ID = 456, Distance = new Distance("25K"), Race = new Race { ID = 456 } };

	public static readonly Result[] Results =
	[
		//complete 100K
		new Result { Athlete = CourseData.Athlete1, Course = Road1, StartTime = new Date(DateTime.Parse("5/20/2024 6am")), Duration = new Time(TimeSpan.FromHours(2)) },
		new Result { Athlete = CourseData.Athlete1, Course = Road2, StartTime = new Date(DateTime.Parse("5/20/2024 11am")), Duration = new Time(TimeSpan.FromHours(2)) },
		new Result { Athlete = CourseData.Athlete1, Course = Trail1, StartTime = new Date(DateTime.Parse("5/20/2024 4pm")), Duration = new Time(TimeSpan.FromHours(2)) },
		new Result { Athlete = CourseData.Athlete1, Course = Trail2, StartTime = new Date(DateTime.Parse("5/20/2024 9pm")), Duration = new Time(TimeSpan.FromHours(2)) },

		//fastest
		new Result { Athlete = CourseData.Athlete1, Course = Road1, StartTime = new Date(DateTime.Parse("5/21/2024 6am")), Duration = new Time(TimeSpan.FromHours(2.5)) },
		new Result { Athlete = CourseData.Athlete1, Course = Road2, StartTime = new Date(DateTime.Parse("5/21/2024 9am")), Duration = new Time(TimeSpan.FromHours(2.5)) },
		new Result { Athlete = CourseData.Athlete1, Course = Trail1, StartTime = new Date(DateTime.Parse("5/21/2024 12pm")), Duration = new Time(TimeSpan.FromHours(2.5)) },
		new Result { Athlete = CourseData.Athlete1, Course = Trail2, StartTime = new Date(DateTime.Parse("5/21/2024 3pm")), Duration = new Time(TimeSpan.FromHours(2.5)) },

		//earliest
		new Result { Athlete = CourseData.Athlete2, Course = Road1, StartTime = new Date(DateTime.Parse("5/20/2024 7am")), Duration = new Time(TimeSpan.FromHours(3)) },
		new Result { Athlete = CourseData.Athlete2, Course = Road2, StartTime = new Date(DateTime.Parse("5/20/2024 11am")), Duration = new Time(TimeSpan.FromHours(3)) },
		new Result { Athlete = CourseData.Athlete2, Course = Trail1, StartTime = new Date(DateTime.Parse("5/20/2024 3pm")), Duration = new Time(TimeSpan.FromHours(3)) },
		new Result { Athlete = CourseData.Athlete2, Course = Trail2, StartTime = new Date(DateTime.Parse("5/20/2024 7pm")), Duration = new Time(TimeSpan.FromHours(3)) },

		//failed 100K, complete 50Ks
		new Result { Athlete = CourseData.Athlete2, Course = Road1, StartTime = new Date(DateTime.Parse("5/21/2024 5am")), Duration = new Time(TimeSpan.FromHours(3.5)) },
		new Result { Athlete = CourseData.Athlete2, Course = Road2, StartTime = new Date(DateTime.Parse("5/21/2024 11am")), Duration = new Time(TimeSpan.FromHours(3.5)) },
		new Result { Athlete = CourseData.Athlete2, Course = Trail1, StartTime = new Date(DateTime.Parse("5/21/2024 5pm")), Duration = new Time(TimeSpan.FromHours(3.5)) },
		new Result { Athlete = CourseData.Athlete2, Course = Trail2, StartTime = new Date(DateTime.Parse("5/21/2024 11pm")), Duration = new Time(TimeSpan.FromHours(3.5)) },

		//incomplete
		new Result { Athlete = CourseData.Athlete2, Course = Trail1, StartTime = new Date(DateTime.Parse("5/22/2024 8am")), Duration = new Time(TimeSpan.FromHours(4)) }
	];
}