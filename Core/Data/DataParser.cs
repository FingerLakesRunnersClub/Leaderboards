using FLRC.Leaderboards.Core.Athletes;

namespace FLRC.Leaderboards.Core.Data;

public static class DataParser
{
	public static Category ParseCategory(string value)
		=> Enum.TryParse<AgeGradeCalculator.Category>(value, true, out var category)
			? new Category(category)
			: null;
}