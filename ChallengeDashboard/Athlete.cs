using System;
using System.Collections.Generic;

namespace ChallengeDashboard
{
    public class Athlete
    {
        public uint ID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public DateTime DateOfBirth { get; set; }
        public IEnumerable<Result> Results { get; set; }

        public byte AgeAsOf(DateTime date)
            => (byte)(date.Subtract(DateOfBirth).TotalDays / 365.2425);
    }
}