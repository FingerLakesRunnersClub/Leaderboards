using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public partial class DataServiceTests
    {
        [Fact]
        public void CanGetCourseNames()
        {
            //arrange
            var api = Substitute.For<IDataAPI>();
            var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var dataService = new DataService(api, config, loggerFactory);

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
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var dataService = new DataService(api, config, loggerFactory);

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
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var dataService = new DataService(api, config, loggerFactory);

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
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var dataService = new DataService(api, config, loggerFactory);

            //act
            await dataService.GetResults(123);
            await dataService.GetResults(123);

            //assert
            await api.Received(1).GetResults(Arg.Any<uint>());
        }

        [Fact]
        public async Task APIExceptionsAreLogged()
        {
            //arrange
            var api = Substitute.For<IDataAPI>();
            api.GetResults(Arg.Any<uint>()).Throws(new Exception());
            var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();

            var logger = new TestLogger();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            loggerFactory.CreateLogger(Arg.Any<string>()).Returns(logger);
            var dataService = new DataService(api, config, loggerFactory);

            //act
            await dataService.GetResults(123);

            //assert
            Assert.True(logger.Called);
        }
    }
}