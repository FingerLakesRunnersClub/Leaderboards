namespace FLRC.Leaderboards.Core.Athletes;

public record Category(FLRC.AgeGradeCalculator.Category Value) : Formatted<FLRC.AgeGradeCalculator.Category>(Value)
{
	public static Category F => new(AgeGradeCalculator.Category.F);
	public static Category M => new(AgeGradeCalculator.Category.M);

	public override string Display => Value.ToString();
}