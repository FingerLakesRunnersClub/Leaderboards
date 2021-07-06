using System;
using System.Collections.Generic;
using System.Linq;

namespace FLRC.ChallengeDashboard
{
    public class TeamMember : IComparable<TeamMember>
    {
        public AgeGrade AgeGrade { get; }
        public ushort Runs { get; }
        public double Miles { get; }

        public TeamMember(IReadOnlyCollection<Ranked<Time>> results)
        {
            AgeGrade = new AgeGrade(results.Average(r => r.AgeGrade.Value));
            Runs = (ushort) results.Sum(r => r.Count);
            Miles = Math.Round(results.Sum(r => r.Count * r.Result.Course.Miles), 1);
        }

        public int CompareTo(TeamMember other) => AgeGrade.CompareTo(other.AgeGrade);
    }
}