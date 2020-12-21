using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class PointsTests
    {
        [Fact]
        public void CanDisplayRoundedPoints()
        {
            //arrange
            var points = new Points(98.765);

            //act
            var display = points.Display;

            //assert
            Assert.Equal("98.77", display);
        }
    }
}