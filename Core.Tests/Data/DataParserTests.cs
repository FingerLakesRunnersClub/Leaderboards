using FLRC.Leaderboards.Core.Data;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Data;

public class DataParserTests
{
	[Theory]
	[InlineData("F", AgeGradeCalculator.Category.F)]
	[InlineData("M", AgeGradeCalculator.Category.M)]
	[InlineData("X", null)]
	public void CanParseCategory(string cat, AgeGradeCalculator.Category? expected)
	{
		//act
		var category = DataParser.ParseCategory(cat);

		//assert
		if (expected != null)
			Assert.Equal(expected, category.Value);
		else
			Assert.Null(category);
	}
}