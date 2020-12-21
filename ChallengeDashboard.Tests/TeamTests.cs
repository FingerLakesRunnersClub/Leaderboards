using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class TeamTests
    {
        [Theory]
        [InlineData(1, "1–19")]
        [InlineData(2, "20–29")]
        [InlineData(7, "70+")]
        public void CanGetTeamDisplayNameFromAge(byte id, string expected)
        {
            //arrange
            var team = new Team(id);

            //act
            var teamName = team.Display;

            //assert
            Assert.Equal(expected, teamName);
        }
    }
}