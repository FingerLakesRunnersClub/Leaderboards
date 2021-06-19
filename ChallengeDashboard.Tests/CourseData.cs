using System;
using System.Collections.Generic;

namespace FLRC.ChallengeDashboard.Tests
{
    public static class CourseData
    {
        public static readonly Athlete Athlete1 = new() {Category = Category.M, Age = 20, DateOfBirth = DateTime.Parse("1/1/2000")};
        public static readonly Athlete Athlete2 = new() {Category = Category.F, Age = 20, DateOfBirth = DateTime.Parse("1/1/2000")};
        public static readonly Athlete Athlete3 = new() {Category = Category.M, Age = 30, DateOfBirth = DateTime.Parse("1/1/1990")};
        public static readonly Athlete Athlete4 = new() {Category = Category.F, Age = 30, DateOfBirth = DateTime.Parse("1/1/1990")};

        public static IEnumerable<Result> Results => new List<Result>
        {
            new() {Athlete = Athlete1, StartTime = new Date(DateTime.Parse("1/1/2020")), Duration = new Time(TimeSpan.Parse("2:34:56.7"))},
            new() {Athlete = Athlete1, StartTime = new Date(DateTime.Parse("1/3/2020")), Duration = new Time(TimeSpan.Parse("1:20:00.0"))},
            new() {Athlete = Athlete2, StartTime = new Date(DateTime.Parse("1/7/2020")), Duration = new Time(TimeSpan.Parse("0:54:32.1"))},
            new() {Athlete = Athlete3, StartTime = new Date(DateTime.Parse("1/5/2020")), Duration = new Time(TimeSpan.Parse("1:02:03.4"))},
            new() {Athlete = Athlete3, StartTime = new Date(DateTime.Parse("1/2/2020")), Duration = new Time(TimeSpan.Parse("1:00:00.0"))},
            new() {Athlete = Athlete4, StartTime = new Date(DateTime.Parse("1/6/2020")), Duration = new Time(TimeSpan.Parse("2:03:04.5"))},
            new() {Athlete = Athlete4, StartTime = new Date(DateTime.Parse("1/8/2020")), Duration = new Time(TimeSpan.Parse("2:22:22.2"))},
            new() {Athlete = Athlete4, StartTime = new Date(DateTime.Parse("1/4/2020")), Duration = new Time(TimeSpan.Parse("2:00:00.0"))}
        };
    }
}