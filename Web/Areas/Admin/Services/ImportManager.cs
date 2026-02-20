using System.Collections.ObjectModel;
using FLRC.Leaderboards.Core.Data;
using Course = FLRC.Leaderboards.Data.Models.Course;

namespace FLRC.Leaderboards.Web.Areas.Admin.Services;

public sealed class ImportManager(ILegacyDataConverter legacyConverter, IResultService resultService, IDictionary<string, IResultsAPI> resultsAPI) : IImportManager
{
	public string[] Sources => resultsAPI.Keys.ToArray();

	public async Task ImportAthletes(string source, uint externalID)
	{
		var importer = resultsAPI[source];
		var response = await importer.GetResults(externalID);
		var athletes = response.GetProperty("Racers").EnumerateArray().Select(j => importer.Source.ParseAthlete(j, ReadOnlyDictionary<string, string>.Empty)).ToArray();
		foreach (var athlete in athletes)
			await legacyConverter.GetAthlete(source, athlete);
	}

	public async Task ImportResults(Course course, string source, uint externalID, DateTime? dateOverride = null)
	{
		var importer = resultsAPI[source];
		var response = await importer.GetResults(externalID);
		var dataSource = importer.Source;
		var legacyResults = dataSource.ParseCourse(null, response, null);
		var results = await legacyConverter.ConvertResults(course.ID, source, legacyResults, dateOverride);
		await resultService.Import(results);
	}
}