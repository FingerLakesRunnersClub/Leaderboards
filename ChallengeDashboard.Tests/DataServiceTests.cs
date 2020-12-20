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
        public void CanGetCourseNames()
        {
            //arrange
            var api = Substitute.For<IDataAPI>();
            var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
            var dataService = new DataService(api, config);

            //act
            var courseNames = dataService.CourseNames;

            //assert
            Assert.Equal("Virgil Crest Ultramarathons", courseNames.First().Value);
        }

        [Fact]
        public async Task CanParseResultsFromAPI()
        {
            //arrange
            var api = Substitute.For<IDataAPI>();
            var json = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
            api.GetResults(Arg.Any<uint>()).Returns(json.RootElement);
            var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
            var dataService = new DataService(api, config);

            //act
            var course = await dataService.GetResults(123);

            //assert
            Assert.Equal("Steve Desmond", course.Results.First().Athlete.Name);
        }


        [Fact]
        public async Task CanGetAllResults()
        {
            //arrange
            var api = Substitute.For<IDataAPI>();
            var json = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
            api.GetResults(Arg.Any<uint>()).Returns(json.RootElement);
            var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
            var dataService = new DataService(api, config);

            //act
            var allResults = await dataService.GetAllResults();

            //assert
            Assert.Equal("Steve Desmond", allResults.First().Results.First().Athlete.Name);
        }

        [Fact]
        public async Task DataFromAPIIsCached()
        {
            //arrange
            var api = Substitute.For<IDataAPI>();
            var json = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
            api.GetResults(Arg.Any<uint>()).Returns(json.RootElement);
            var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
            var dataService = new DataService(api, config);

            //act
            await dataService.GetResults(123);
            await dataService.GetResults(123);

            //assert
            await api.Received(1).GetResults(Arg.Any<uint>());
        }
    }
}