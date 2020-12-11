using System.Linq;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class RankedExtensionTests
    {
        [Fact]
        public void CanRankResults()
        {
            //arrange
            var results = TestData.Results.GroupBy(r => r.Athlete).Select(g => new GroupedResult(g));

            //act
            var ranked = results.Rank(rs => rs.Min(r => r.Duration)).ToArray();

            //assert
            Assert.True(ranked[0].Value < ranked[1].Value);
            Assert.True(ranked[1].Value < ranked[2].Value);
            Assert.True(ranked[2].Value < ranked[3].Value);
        }

        [Fact]
        public void CanRankResultsInDescendingOrder()
        {
            //arrange
            var results = TestData.Results.GroupBy(r => r.Athlete).Select(g => new GroupedResult(g));

            //act
            var ranked = results.RankDescending(rs => rs.Min(r => r.Duration)).ToArray();

            //assert
            Assert.True(ranked[0].Value > ranked[1].Value);
            Assert.True(ranked[1].Value > ranked[2].Value);
            Assert.True(ranked[2].Value > ranked[3].Value);
        }

        [Fact]
        public void FastestTimeBreaksTies()
        {
            //arrange
            var results = TestData.Results.GroupBy(r => r.Athlete).Select(g => new GroupedResult(g));

            //act
            var ranked = results.Rank(rs => rs.Count()).ToArray();

            //assert
            Assert.Equal(ranked[2].Value, ranked[1].Value);
            Assert.Equal(TestData.Athlete3, ranked[1].Athlete);
            Assert.Equal(TestData.Athlete1, ranked[2].Athlete);
        }
    }
}