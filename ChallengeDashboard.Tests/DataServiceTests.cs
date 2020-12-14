using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
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
            var config = Substitute.For<IConfiguration>();
            var dataService = new DataService(api, config);

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
            var config = Substitute.For<IConfigurationRoot>();
            var configSection = new ConfigurationSection(config, "Courses") { Value = "123" };
            config.GetSection("Courses").Returns(configSection);
            var dataService = new DataService(api, config);

            //act
            var courses = await dataService.GetAllCourses();

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
            var config = Substitute.For<IConfiguration>();
            var dataService = new DataService(api, config);

            //act
            await dataService.GetCourse(123);
            await dataService.GetCourse(123);

            //assert
            await api.Received(1).GetCourse(Arg.Any<uint>());
        }
    }
}