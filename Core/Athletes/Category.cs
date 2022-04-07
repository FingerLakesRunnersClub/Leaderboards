namespace FLRC.Leaderboards.Core.Athletes;

public record Category(AgeGradeCalculator.Category Value) : Formatted<AgeGradeCalculator.Category>(Value)
{
	public static Category F => new(AgeGradeCalculator.Category.F);
	public static Category M => new(AgeGradeCalculator.Category.M);

	public override string Display => Value.ToString();

	public static Category Parse(string value)
		=> Enum.TryParse<AgeGradeCalculator.Category>(value, true, out var category)
			? new Category(category)
			: null;
}