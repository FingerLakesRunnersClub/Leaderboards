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
            Assert.StartsWith("20", teamName);
            Assert.EndsWith("29", teamName);
        }
    }
}