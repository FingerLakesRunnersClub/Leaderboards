using Xunit;

namespace FLRC.ChallengeDashboard.Tests;

public class CategoryTests
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
}
