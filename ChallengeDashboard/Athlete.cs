using System;
using System.Collections.Generic;
using System.Linq;

namespace FLRC.ChallengeDashboard
{
    public class Athlete
    {
        public uint ID { get; init; }
        public string Name { get; init; }

        public Category Category { get; init; }

        public byte Age { get; init; }
        public DateTime DateOfBirth { get; init; }

        public byte AgeAsOf(DateTime date) => (byte)(date.Subtract(DateOfBirth).TotalDays / 365.2425);

        public Team Team => Age < 20 ? Teams[1]
            : Age >= 80 ? Teams[7]
            : Teams[(byte)(Age / 10)];

        public static IDictionary<byte, Team> Teams = Enumerable.Range(1, 7).ToDictionary(t => (byte)t, t => new Team((byte)t));
    }
}