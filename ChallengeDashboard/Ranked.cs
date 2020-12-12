using System;

namespace FLRC.ChallengeDashboard
{
    public class Ranked<T>
    {
        public ushort Rank { get; set; }
        public Athlete Athlete { get; set; }
        public Result Result{ get; set; }
        public T Value { get; set; }

        public TimeSpan BehindLeader { get; set; }
        public string BehindLeaderDisplay => BehindLeader.ToString(Result.TimeFormat);

        public double AgeGrade { get; set; }
        public string AgeGradeDisplay => AgeGrade > 0 ? AgeGrade.ToString("F1") + "%" : string.Empty;
    }
}