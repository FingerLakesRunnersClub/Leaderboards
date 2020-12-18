namespace FLRC.ChallengeDashboard
{
    public class TeamResults
    {
        public byte Rank { get; internal set; }
        public Team Team { get; set; }

        public AgeGrade AverageAgeGrade { get; set; }
        public double AgeGradePoints { get; set; }

        public ushort TotalRuns { get; set; }
        public byte MostRunsPoints { get; set; }

        public byte TotalPoints => (byte)(AgeGradePoints + MostRunsPoints);
    }
}