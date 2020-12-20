using System;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class RankedTests
    {
        [Fact]
        public void CanGetPointsFromTimeBehindLeader()
        {
            //arrange
            var result = new Ranked<Time>
            {
                Result = new Result
                {
                    Duration = new Time(new TimeSpan(1, 20, 0))
                },
                BehindLeader = new Time(new TimeSpan(0, 20, 0))
            };

            //act
            var points = result.Points;

            //assert
            Assert.Equal(75, points);
        }
    }
}