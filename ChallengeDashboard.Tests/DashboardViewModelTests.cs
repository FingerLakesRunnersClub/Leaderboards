using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class DashboardViewModelTests
    {
        [Fact]
        public void TitleIsStatic()
        {
            //arrange
            var vm = new DashboardViewModel(new List<Course>());
            
            //act
            var title = vm.Title;
            
            //assert
            Assert.Equal("Dashboard", title);
        }

        [Fact]
        public void CanGetCourseNamesFromCourses()
        {
            //arrange
            var courses = new List<Course> {new Course {Name = "Virgil Crest Ultramarathons"}};
            var vm = new DashboardViewModel(courses);
            
            //act
            var names = vm.CourseNames;
            
            //assert
            Assert.Equal("Virgil Crest Ultramarathons", names.First().Value);
        }
        
        [Fact]
        public void CourseResultsContainsAllTables()
        {
            //arrange
            var vm = new DashboardViewModel(DashboardData.Courses);

            //act
            var results = vm.CourseResults;
            
            //assert
            var result = results.First().Value;
            Assert.Equal("1:02:03.0", result.First(r => r.Category.Equals(Category.M) && r.ResultType.Value == ResultType.Fastest).Rows.First().Value);
            Assert.StartsWith("2:", result.First(r => r.Category.Equals(Category.M) && r.ResultType.Value == ResultType.BestAverage).Rows.First().Value);
            Assert.Equal("3:02:01.0", result.First(r => r.Category.Equals(Category.F) && r.ResultType.Value == ResultType.Fastest).Rows.First().Value);
            Assert.StartsWith("4:", result.First(r => r.Category.Equals(Category.F) && r.ResultType.Value == ResultType.BestAverage).Rows.First().Value);
            Assert.StartsWith("20", result.First(r => r.ResultType.Value == ResultType.Team && r.Title.StartsWith("Fastest")).Rows.First().Name);
            Assert.StartsWith("30", result.First(r => r.ResultType.Value == ResultType.Team && r.Title.StartsWith("Most")).Rows.First().Name);
        }

        [Fact]
        public void OverallResultsContainsAllTables()
        {
            //arrange
            var vm = new DashboardViewModel(DashboardData.Courses);
            
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