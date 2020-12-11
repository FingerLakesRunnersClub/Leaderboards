using System;

namespace FLRC.ChallengeDashboard
{
    public class Ranked<T>
    {
        public ushort Rank { get; set; }
        public Athlete Athlete { get; set; }
        public T Value { get; set; }
        public TimeSpan BehindLeader { get; set; }
        public string BehindLeaderDisplay => BehindLeader.ToString(Result.TimeFormat);
    }
}