namespace FLRC.ChallengeDashboard
{
    public class SimilarAthlete
    {
        public Rank Rank { get; set; }
        public Athlete Athlete { get; init; }
        public Percent Similarity { get; init; }
        public Percent Confidence { get; init; }
        public SpeedComparison FastestPercent { get; init; }
        public SpeedComparison AveragePercent { get; init; }
    }
}