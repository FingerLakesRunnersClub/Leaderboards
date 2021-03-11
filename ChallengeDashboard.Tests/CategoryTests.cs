using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class CategoryTests
    {
        [Theory]
        [InlineData(AgeGradeCalculator.Category.F, "F")]
        [InlineData(AgeGradeCalculator.Category.M, "M")]
        [InlineData(null, null)]
        public void CanDisplayCategory(AgeGradeCalculator.Category? cat, string expected)
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
            Assert.NotEqual(Category.M, Category.F);
        }

        [Fact]
        public void CanEquateCategories()
        {
            //arrange
            var f1 = new Category(AgeGradeCalculator.Category.F);
            var f2 = new Category(AgeGradeCalculator.Category.F);

            //act
            var equal = f1.Equals(f2);
            
            //assert
            Assert.True(equal);
        }
    }
}