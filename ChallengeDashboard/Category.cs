namespace FLRC.ChallengeDashboard
{
    public record Category : Formatted<FLRC.AgeGradeCalculator.Category>
    {
        public static Category F => new (AgeGradeCalculator.Category.F);
        public static Category M => new (AgeGradeCalculator.Category.M);

        public Category(FLRC.AgeGradeCalculator.Category value) : base(value)
        {
        }

        public override string Display => Value.ToString();
    }
}