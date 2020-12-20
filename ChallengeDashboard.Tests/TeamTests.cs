using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class TeamTests
    {
        [Fact]
        public void CanGetTeamDisplayNameFromAge()
        {
            //arrange
            var team = new Team(2);

            //act
            var teamName = team.Display;

            //assert
            Assert.Equal("20–29", teamName);
        }
    }
}