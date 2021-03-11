using System;

namespace FLRC.ChallengeDashboard
{
    public class Category : Formatted<FLRC.AgeGradeCalculator.Category?>, IEquatable<Category>
    {
        public static Category F => new (AgeGradeCalculator.Category.F);
        public static Category M => new (AgeGradeCalculator.Category.M);

        public Category(FLRC.AgeGradeCalculator.Category? value) : base(value)
        {
        }

        public override string Display => Value?.ToString();

        public bool Equals(Category other) => Value.Equals(other?.Value);
    }
}