using System;

namespace FLRC.ChallengeDashboard
{
    public class Ranked<T>
    {
        public ushort Rank { get; set; }
        public Athlete Athlete { get; set; }
        public Result Result { get; set; }
        public T Value { get; set; }
        public uint Count { get; set; }

        public AgeGrade AgeGrade { get; set; }

        public Time BehindLeader { get; set; }

        public int Points => (int)(Result.Duration.Subtract(BehindLeader).Value.TotalSeconds /
            Result.Duration.Value.TotalSeconds * 100);
    }
}