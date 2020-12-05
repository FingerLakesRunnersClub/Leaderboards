using System;

namespace ChallengeDashboard
{
    public class Athlete
    {
        public uint ID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public byte Age { get; set; }
        public DateTime DateOfBirth { get; set; }

        public byte AgeAsOf(DateTime date) => (byte) (date.Subtract(DateOfBirth).TotalDays / 365.2425);

        public byte Team => (byte)(Age / 10 * 10);

    }
}