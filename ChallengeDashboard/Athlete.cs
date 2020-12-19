using System;
using System.Collections.Generic;
using System.Linq;

namespace FLRC.ChallengeDashboard
{
    public class Athlete
    {
        public uint ID { get; set; }
        public string Name { get; set; }

        public Category Category { get; set; }

        public byte Age { get; set; }
        public DateTime DateOfBirth { get; set; }

        public byte AgeAsOf(DateTime date) => (byte)(date.Subtract(DateOfBirth).TotalDays / 365.2425);

        public Team Team => Age < 20 ? Teams[2]
            : Age >= 80 ? Teams[7]
            : Teams[(byte)(Age / 10)];

        public static IDictionary<byte, Team> Teams = Enumerable.Range(2, 6).ToDictionary(t => (byte)t, t => new Team((byte)t));
    }
}