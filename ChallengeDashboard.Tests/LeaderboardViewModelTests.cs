using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class LeaderboardViewModelTests
    {
        [Fact]
        public void TitleIsStatic()
        {
            //arrange
            var vm = new LeaderboardViewModel(new List<Course>(), LeaderboardResultType.Team);
            
            //act
            var title = vm.Title;
            
            //assert
            Assert.Equal("Leaderboard", title);
        }

        [Fact]
        public void CanGetCourseNamesFromCourses()
        {
            //arrange
            var courses = new List<Course> {new Course {Name = "Virgil Crest Ultramarathons"}};
            var vm = new LeaderboardViewModel(courses, LeaderboardResultType.Team);
            
            //act
            var names = vm.CourseNames;
            
            //assert
            Assert.Equal("Virgil Crest Ultramarathons", names.First().Value);
        }
        
        [Fact]
        public void TeamResultsContainsAllTables()
        {
            //arrange
            var vm = new LeaderboardViewModel(LeaderboardData.Courses, LeaderboardResultType.Team);

            //act
            var results = vm.CourseResults;
            
            //assert
            var result = results.First().Value.ToList();
            Assert.StartsWith("20", result.First(r => r.ResultType.Value == ResultType.Team && r.Title.StartsWith("Fastest")).Rows.First().Name);
            Assert.StartsWith("30", result.First(r => r.ResultType.Value == ResultType.Team && r.Title.StartsWith("Most")).Rows.First().Name);
            Assert.StartsWith("20", result.First(r => r.ResultType.Value == ResultType.Team && r.Title.StartsWith("Total")).Rows.First().Name);
        }
        
        [Fact]
        public void FResultsContainsAllTables()
        {
            //arrange
            var vm = new LeaderboardViewModel(LeaderboardData.Courses, LeaderboardResultType.F);

            //act
            var results = vm.CourseResults;
            
            //assert
            var result = results.First().Value.ToList();
            Assert.Equal("3:02:01.0", result.First(r => r.Category.Equals(Category.F) && r.ResultType.Value == ResultType.Fastest).Rows.First().Value);
            Assert.StartsWith("4:", result.First(r => r.Category.Equals(Category.F) && r.ResultType.Value == ResultType.BestAverage).Rows.First().Value);
            Assert.StartsWith("3", result.First(r => r.ResultType.Value == ResultType.MostRuns).Rows.First().Value);
        }
        
        [Fact]
        public void MResultsContainsAllTables()
        {
            //arrange
            var vm = new LeaderboardViewModel(LeaderboardData.Courses, LeaderboardResultType.M);

            //act
            var results = vm.CourseResults;
            
            //assert
            var result = results.First().Value.ToList();
            Assert.Equal("1:02:03.0", result.First(r => r.Category.Equals(Category.M) && r.ResultType.Value == ResultType.Fastest).Rows.First().Value);
            Assert.StartsWith("2:", result.First(r => r.Category.Equals(Category.M) && r.ResultType.Value == ResultType.BestAverage).Rows.First().Value);
            Assert.StartsWith("3", result.First(r => r.ResultType.Value == ResultType.MostRuns).Rows.First().Value);
        }

        [Fact]
        public void OverallResultsContainsAllTables()
        {
            //arrange
            var vm = new LeaderboardViewModel(LeaderboardData.Courses, LeaderboardResultType.Team);
            
            //act
            var results = vm.OverallResults;
            
            //assert
            Assert.Equal("A3", results.First(r => r.Title == "Most Points (F)").Rows.First().Name);
            Assert.Equal("A1", results.First(r => r.Title == "Most Points (M)").Rows.First().Name);
            Assert.Equal("A2", results.First(r => r.Title == "Most Miles").Rows.First().Name);
            Assert.StartsWith("20", results.First(r => r.Title == "Top Teams").Rows.First().Name);
        }
    }
}