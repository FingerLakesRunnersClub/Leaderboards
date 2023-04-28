using FLRC.Leaderboards.Core.Athletes;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Athletes;

public sealed class CategoryTests
{
	[Theory]
	[InlineData(AgeGradeCalculator.Category.F, "F")]
	[InlineData(AgeGradeCalculator.Category.M, "M")]
	public void CanDisplayCategory(AgeGradeCalculator.Category cat, string expected)
	{
		//arrange
		var category = new Category(cat);

		//act
		var display = category.Display;

		//assert
		Assert.Equal(expected, display);
	}

	[Fact]
	public void CanCompareCategories()
	{
		//arrange
		var f = Category.F;

		//act
		var m = Category.M;

		//assert
		Assert.Equal(Category.M, m);
		Assert.Equal(Category.F, f);
		Assert.NotEqual(m, f);
	}

	[Fact]
	public void CanEquateCategories()
	{
		//arrange
		var f1 = new Category(AgeGradeCalculator.Category.F);

		//act
		var f2 = new Category(AgeGradeCalculator.Category.F);

		//assert
		Assert.Equal(f1, f2);
	}

	[Theory]
	[InlineData("F", AgeGradeCalculator.Category.F)]
	[InlineData("M", AgeGradeCalculator.Category.M)]
	[InlineData("X", null)]
	public void CanParseCategory(string cat, AgeGradeCalculator.Category? expected)
	{
		//act
		var category = Category.Parse(cat);

		//assert
		if (expected != null)
			Assert.Equal(expected, category.Value);
		else
			Assert.Null(category);
	}
}