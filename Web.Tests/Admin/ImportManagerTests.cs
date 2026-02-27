using System.Text.Json;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Web.Areas.Admin.Services;
using NSubstitute;
using Xunit;
using Athlete = FLRC.Leaderboards.Core.Athletes.Athlete;

namespace FLRC.Leaderboards.Web.Tests.Admin;

public sealed class ImportManagerTests
{
	[Fact]
	public async Task CanImportAthletesFromLegacyData()
	{
		//arrange
		var converter = Substitute.For<ILegacyDataConverter>();
		var service = Substitute.For<IResultService>();
		var results = Substitute.For<IResultsAPI>();
		var api = new Dictionary<string, IResultsAPI> { { nameof(WebScorer), results } };
		var manager = new ImportManager(converter, service, api);

		results.GetResults(123).Returns(JsonElement.Parse(@"{""Racers"":[{},{}]}"));

		//act
		await manager.ImportAthletes(nameof(WebScorer), 123);

		//assert
		await converter.Received(2).GetAthlete(nameof(WebScorer), Arg.Any<Athlete>());
	}

	[Fact]
	public async Task CanImportResultsFromLegacyData()
	{
		//arrange
		var converter = Substitute.For<ILegacyDataConverter>();
		var service = Substitute.For<IResultService>();
		var results = Substitute.For<IResultsAPI>();
		results.Source.Returns(new WebScorer(Substitute.For<IConfig>()));
		var api = new Dictionary<string, IResultsAPI> { { nameof(WebScorer), results } };
		var manager = new ImportManager(converter, service, api);

		var data = await File.ReadAllTextAsync("json/athlete.json");
		results.GetResults(123).Returns(JsonElement.Parse(data));
		converter.ConvertResults(Arg.Any<Guid>(), nameof(WebScorer), Arg.Any<Core.Results.Result[]>(), null).Returns([
			new Result(),
			new Result()
		]);

		//act
		await manager.ImportResults(new Course(), nameof(WebScorer), 123);

		//assert
		await service.Received().Import(Arg.Is<Result[]>(r => r.Length == 2));
	}
}