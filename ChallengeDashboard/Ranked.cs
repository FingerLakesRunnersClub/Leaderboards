namespace FLRC.ChallengeDashboard
{
    public class Ranked<T>
    {
        public ushort Rank { get; init; }
        public Athlete Athlete { get; init; }
        public Result Result { get; init; }
        public T Value { get; init; }
        public uint Count { get; init; }

        public AgeGrade AgeGrade { get; init; }

        public Time BehindLeader { get; init; }

        public int Points => (int)(Result.Duration.Subtract(BehindLeader).Value.TotalSeconds / Result.Duration.Value.TotalSeconds * 100);
    }
}