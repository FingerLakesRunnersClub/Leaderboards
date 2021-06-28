using System.Linq;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class OverallResultsTests
    {
        [Fact]
        public void CanGetMostPoints()
        {
            //arrange
            var vm = new OverallResults(LeaderboardData.Courses);
            
            //act
            var mostPoints = vm.MostPoints();

            //assert
            Assert.Equal(LeaderboardData.Athlete1, mostPoints.First().Result.Athlete);
        }
        
        [Fact]
        public void CanGetMostPointsForCategory()
        {
            //arrange
            var vm = new OverallResults(LeaderboardData.Courses);
            
            //act
            var mostPoints = vm.MostPoints(Category.F);

            //assert
            Assert.Equal(LeaderboardData.Athlete3, mostPoints.First().Result.Athlete);
        }
        
        [Fact]
        public void CanGetMostMiles()
        {
            //arrange
            var vm = new OverallResults(LeaderboardData.Courses);
            
            //act
            var mostPoints = vm.MostMiles();

            //assert
            Assert.Equal(LeaderboardData.Athlete2, mostPoints.First().Result.Athlete);
        }
        
        [Fact]
        public void CanGetMostMilesForCategory()
        {
            //arrange
            var vm = new OverallResults(LeaderboardData.Courses);
            
            //act
            var mostPoints = vm.MostMiles(Category.F);

            //assert
            Assert.Equal(LeaderboardData.Athlete4, mostPoints.First().Result.Athlete);
        }
        
        [Fact]
        public void CanGetBestAverageAgeGrade()
        {
            //arrange
            var vm = new OverallResults(LeaderboardData.Courses);
            
            //act
            var mostPoints = vm.AgeGrade();

            //assert
            Assert.Equal(LeaderboardData.Athlete1, mostPoints.First().Result.Athlete);
        }
        
        [Fact]
        public void CanGetBestAverageAgeGradeForCategory()
        {
            //arrange
            var vm = new OverallResults(LeaderboardData.Courses);
            
            //act
            var mostPoints = vm.AgeGrade(Category.F);

            //assert
            Assert.Equal(LeaderboardData.Athlete3, mostPoints.First().Result.Athlete);
        }
        
        [Fact]
        public void CanGetTeamPoints()
        {
            //arrange
            var vm = new OverallResults(LeaderboardData.Courses);
            
            //act
            var mostPoints = vm.TeamPoints();

            //assert
            Assert.Equal("1–29", mostPoints.First().Team.Display);
        }
        
        [Fact]
        public void CanGetTeamMembers()
        {
            //arrange
            var vm = new OverallResults(LeaderboardData.Courses);
            
            //act
            var members = vm.TeamMembers(2);

            //assert
            Assert.Equal("1–29", members.First().Result.Athlete.Team.Display);
        }
    }
}