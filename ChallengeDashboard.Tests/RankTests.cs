using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class RankTests
    {
        [Theory]
        [InlineData(0, "0")]
        [InlineData(1, "1st")]
        [InlineData(2, "2nd")]
        [InlineData(3, "3rd")]
        [InlineData(4, "4th")]
        [InlineData(11, "11th")]
        [InlineData(12, "12th")]
        [InlineData(13, "13th")]
        public void CanDisplayOrdinal(ushort value, string expected)
        {
            //arrange
            var rank = new Rank(value);
            
            //act
            var display = rank.Display;
            
            //assert
            Assert.Equal(expected, display);
        }
    }
}