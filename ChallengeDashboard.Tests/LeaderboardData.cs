using System;
using System.Collections.Generic;

namespace FLRC.ChallengeDashboard.Tests
{
    public static class LeaderboardData
    {
        public static readonly Athlete Athlete1 = new() {ID = 123, Name = "A1", Age = 20, Category = Category.M};
        public static readonly Athlete Athlete2 = new() {ID = 234, Name = "A2", Age = 30, Category = Category.M};
        public static readonly Athlete Athlete3 = new() {ID = 345, Name = "A3", Age = 20, Category = Category.F};
        public static readonly Athlete Athlete4 = new() {ID = 456, Name = "A4", Age = 30, Category = Category.F};

        public static readonly IEnumerable<Course> Courses = new List<Course>
        {
            new() {Meters = 10 * Course.MetersPerMile, Results = Results}
        };

        private static Course Course => new() {Meters = 10 * Course.MetersPerMile};

        private static IEnumerable<Result> Results
            => new List<Result>
            {
                new() {Course = Course, Athlete = Athlete1, Duration = new Time(new TimeSpan(1, 2, 3))},
                new() {Course = Course, Athlete = Athlete1, Duration = new Time(new TimeSpan(1, 23, 45))},
                new() {Course = Course, Athlete = Athlete2, Duration = new Time(new TimeSpan(2, 3, 4))},
                new() {Course = Course, Athlete = Athlete2, Duration = new Time(new TimeSpan(2, 34, 56))},
                new() {Course = Course, Athlete = Athlete2, Duration = new Time(new TimeSpan(2, 22, 22))},
                new() {Course = Course, Athlete = Athlete3, Duration = new Time(new TimeSpan(3, 2, 1))},
                new() {Course = Course, Athlete = Athlete3, Duration = new Time(new TimeSpan(3, 21, 0))},
                new() {Course = Course, Athlete = Athlete4, Duration = new Time(new TimeSpan(4, 3, 2))},
                new() {Course = Course, Athlete = Athlete4, Duration = new Time(new TimeSpan(4, 32, 10))},
                new() {Course = Course, Athlete = Athlete4, Duration = new Time(new TimeSpan(4, 4, 4))}
            };
    }
}