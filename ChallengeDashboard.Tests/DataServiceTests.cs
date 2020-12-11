using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;

namespace ChallengeDashboard.Tests
{
    public class DataServiceTests
    {
        [Fact]
        public async Task CanParseCourseFromAPI()
        {
            //arrange
            var api = Substitute.For<IDataAPI>();
            var json = await JsonDocument.ParseAsync(File.OpenRead("json/empty.json"));
            api.GetCourse(Arg.Any<uint>()).Returns(json.RootElement);
            var dataService = new DataService(api);

            //act
            var course = await dataService.GetCourse(123);

            //assert
            Assert.Equal("Virgil Crest Ultramarathons", course.Name);
        }


        [Fact]
        public async Task CanGetAllCourses()
        {
            //arrange
            var api = Substitute.For<IDataAPI>();
            var json = await JsonDocument.ParseAsync(File.OpenRead("json/empty.json"));
            api.GetCourse(Arg.Any<uint>()).Returns(json.RootElement);
            var dataService = new DataService(api);

            //act
            var courses = await dataService.GetAllCourses(new List<uint>() { 123 });

            //assert
            Assert.Equal("Virgil Crest Ultramarathons", courses.First().Name);
        }

        [Fact]
        public async Task DataFromAPIIsCached()
        {
            //arrange
            var api = Substitute.For<IDataAPI>();
            var json = await JsonDocument.ParseAsync(File.OpenRead("json/empty.json"));
            api.GetCourse(Arg.Any<uint>()).Returns(json.RootElement);
            var dataService = new DataService(api);

            //act
            var course1 = await dataService.GetCourse(123);
            var course2 = await dataService.GetCourse(123);

            //assert
            await api.Received(1).GetCourse(Arg.Any<uint>());
        }
    }
}