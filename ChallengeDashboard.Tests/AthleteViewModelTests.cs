using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class AthleteViewModelTests
    {
        [Fact]
        public void TitleIsAthleteName()
        {
            //arrange
            var vm = new AthleteViewModel { Athlete = new Athlete { Name = "Steve Desmond" } };

            //act
            var title = vm.Title;

            //assert
            Assert.Equal("Steve Desmond", title);
        }
    }
}