using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class ResultsViewModelTests
    {
        [Fact]
        public void CanGetTitle()
        {
            //arrange
            var vm = new ResultsViewModel
            {
                ResultType = new FormattedResultType(ResultType.Fastest),
                Course = new Course { Name = "Virgil Crest Ultramarathons" }
            };

            //act
            var title = vm.Title;

            //assert
            Assert.Equal("Fastest — Virgil Crest Ultramarathons", title);
        }
    }
}