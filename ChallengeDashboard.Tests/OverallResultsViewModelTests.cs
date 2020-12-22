using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class OverallResultsViewModelTests
    {
        [Fact]
        public void CanGetTitle()
        {
            //arrange
            var vm = new OverallResultsViewModel { ResultType = "Unit Test"};
            
            //act
            var title = vm.Title;
            
            //assert
            Assert.Equal("Unit Test", title);
        }
    }
}