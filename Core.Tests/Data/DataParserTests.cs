using System.Text.Json;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Metrics;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Data;

public class DataParserTests
{
	[Theory]
	[InlineData("1 mile", Course.MetersPerMile)]
	[InlineData("2 miles", 2 * Course.MetersPerMile)]
	[InlineData("3 mi", 3 * Course.MetersPerMile)]
	[InlineData("1000 m", 1000)]
	[InlineData("2 km", 2000)]
	[InlineData("3 K", 3000)]
	public void CanParseDistance(string distance, double expected)
	{
		//act
		var actual = DataParser.ParseDistance(distance);

		//assert
		Assert.Equal(expected, actual);
	}

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