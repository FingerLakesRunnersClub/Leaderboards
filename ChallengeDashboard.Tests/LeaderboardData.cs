using System;
using System.Collections.Generic;

namespace FLRC.ChallengeDashboard.Tests
{
    public static class LeaderboardData
    {
        public static Athlete Athlete1 = new Athlete { ID = 123, Name = "A1", Age = 20, Category = Category.M };
        public static Athlete Athlete2 = new Athlete { ID = 234, Name = "A2", Age = 30, Category = Category.M };
        public static Athlete Athlete3 = new Athlete { ID = 345, Name = "A3", Age = 20, Category = Category.F };
        public static Athlete Athlete4 = new Athlete { ID = 456, Name = "A4", Age = 30, Category = Category.F };
        public static IList<Course> Courses = new List<Course>
        {
            new Course
            {
                Meters = 10 * Course.MetersPerMile,
                Results = new List<Result>
                {
                    new Result { Athlete = Athlete1, Duration = new Time(new TimeSpan(1, 2, 3))},
                    new Result { Athlete = Athlete1, Duration = new Time(new TimeSpan(1, 23, 45))},
                    new Result { Athlete = Athlete2, Duration = new Time(new TimeSpan(2, 3, 4))},
                    new Result { Athlete = Athlete2, Duration = new Time(new TimeSpan(2, 34, 56))},
                    new Result { Athlete = Athlete2, Duration = new Time(new TimeSpan(2, 22, 22))},
                    new Result { Athlete = Athlete3, Duration = new Time(new TimeSpan(3, 2, 1))},
                    new Result { Athlete = Athlete3, Duration = new Time(new TimeSpan(3, 21, 0))},
                    new Result { Athlete = Athlete4, Duration = new Time(new TimeSpan(4, 3, 2))},
                    new Result { Athlete = Athlete4, Duration = new Time(new TimeSpan(4, 32, 10))},
                    new Result { Athlete = Athlete4, Duration = new Time(new TimeSpan(4, 4, 4))}
                }
            }
        };
    }
}