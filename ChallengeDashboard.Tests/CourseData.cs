using System;
using System.Collections.Generic;

namespace FLRC.ChallengeDashboard.Tests
{
    public static class CourseData
    {
        public static readonly Athlete Athlete1 = new() {Category = Category.M, Age = 20};
        public static readonly Athlete Athlete2 = new() {Category = Category.F, Age = 20};
        public static readonly Athlete Athlete3 = new() {Category = Category.M, Age = 30};
        public static readonly Athlete Athlete4 = new() {Category = Category.F, Age = 30};

        public static readonly IEnumerable<Result> Results = new List<Result>
        {
            new() {Athlete = Athlete1, Duration = new Time(TimeSpan.Parse("2:34:56.7"))},
            new() {Athlete = Athlete1, Duration = new Time(TimeSpan.Parse("1:20:00.0"))},
            new() {Athlete = Athlete2, Duration = new Time(TimeSpan.Parse("0:54:32.1"))},
            new() {Athlete = Athlete3, Duration = new Time(TimeSpan.Parse("1:02:03.4"))},
            new() {Athlete = Athlete3, Duration = new Time(TimeSpan.Parse("1:00:00.0"))},
            new() {Athlete = Athlete4, Duration = new Time(TimeSpan.Parse("2:03:04.5"))},
            new() {Athlete = Athlete4, Duration = new Time(TimeSpan.Parse("2:22:22.2"))},
            new() {Athlete = Athlete4, Duration = new Time(TimeSpan.Parse("2:00:00.0"))}
        };
    }
}