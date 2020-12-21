using System;
using System.Collections.Generic;

namespace FLRC.ChallengeDashboard.Tests
{
    public static class CourseData
    {
        public static readonly Athlete Athlete1 = new Athlete { Category = Category.M };
        public static readonly Athlete Athlete2 = new Athlete { Category = Category.F };
        public static readonly Athlete Athlete3 = new Athlete { Category = Category.M };
        public static readonly Athlete Athlete4 = new Athlete { Category = Category.F };

        public static readonly IEnumerable<Result> Results = new List<Result>()
        {
            new Result { Athlete = Athlete1, Duration = new Time(TimeSpan.Parse("1:20:00.0")) },
            new Result { Athlete = Athlete1, Duration = new Time(TimeSpan.Parse("2:34:56.7")) },
            new Result { Athlete = Athlete2, Duration = new Time(TimeSpan.Parse("0:54:32.1")) },
            new Result { Athlete = Athlete3, Duration = new Time(TimeSpan.Parse("1:02:03.4")) },
            new Result { Athlete = Athlete3, Duration = new Time(TimeSpan.Parse("1:00:00.0")) },
            new Result { Athlete = Athlete4, Duration = new Time(TimeSpan.Parse("2:03:04.5")) },
            new Result { Athlete = Athlete4, Duration = new Time(TimeSpan.Parse("2:22:22.2")) },
            new Result { Athlete = Athlete4, Duration = new Time(TimeSpan.Parse("2:00:00.0")) }
        };
    }
}