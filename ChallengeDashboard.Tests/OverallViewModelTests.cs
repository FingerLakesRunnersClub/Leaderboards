using System.Linq;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class OverallViewModelTests
    {
        [Fact]
        public void CanGetMostPoints()
        {
            //arrange
            var vm = new OverallViewModel(LeaderboardData.Courses);
            
            //act
            var mostPoints = vm.MostPoints();

            //assert
            Assert.Equal(LeaderboardData.Athlete1, mostPoints.First().Athlete);
        }
        
        [Fact]
        public void CanGetMostPointsForCategory()
        {
            //arrange
            var vm = new OverallViewModel(LeaderboardData.Courses);
            
            //act
            var mostPoints = vm.MostPoints(Category.F);

            //assert
            Assert.Equal(LeaderboardData.Athlete3, mostPoints.First().Athlete);
        }
        
        [Fact]
        public void CanGetMostMiles()
        {
            //arrange
            var vm = new OverallViewModel(LeaderboardData.Courses);
            
            //act
            var mostPoints = vm.MostMiles();

            //assert
            Assert.Equal(LeaderboardData.Athlete2, mostPoints.First().Athlete);
        }
        
        [Fact]
        public void CanGetMostMilesForCategory()
        {
            //arrange
            var vm = new OverallViewModel(LeaderboardData.Courses);
            
            //act
            var mostPoints = vm.MostMiles(Category.F);

            //assert
            Assert.Equal(LeaderboardData.Athlete4, mostPoints.First().Athlete);
        }
        
        [Fact]
        public void CanGetTeamPoints()
        {
            //arrange
            var vm = new OverallViewModel(LeaderboardData.Courses);
            
            //act
            var mostPoints = vm.TeamPoints();

            //assert
            Assert.Equal("20–29", mostPoints.First().Team.Display);
        }
    }
}