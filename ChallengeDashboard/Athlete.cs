using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace FLRC.ChallengeDashboard
{
    public class Athlete
    {
        public uint ID { get; init; }
        public string Name { get; init; }

        public Category Category { get; init; }

        public byte Age { get; init; }
        
        [JsonIgnore]
        public DateTime DateOfBirth { get; init; }

        public byte AgeAsOf(DateTimeOffset date) => (byte) (date.Subtract(DateOfBirth).TotalDays / 365.2425);

        public Team Team => Age < 20 ? Teams[2]
            : Age >= 70 ? Teams[6]
            : Teams[(byte) (Age / 10)];

        public byte AgeToday => AgeAsOf(DateTime.Today);

        public static readonly IDictionary<byte, Team> Teams = Enumerable.Range(2, 6).ToDictionary(t => (byte) t, t => new Team((byte) t));
    }
}