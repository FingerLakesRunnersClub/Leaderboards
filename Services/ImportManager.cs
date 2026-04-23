using System.Collections.ObjectModel;
using System.Text.Json;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Model;
using Course = FLRC.Leaderboards.Model.Course;

namespace FLRC.Leaderboards.Services;

public sealed class ImportManager(ILegacyDataConverter legacyConverter, IResultService resultService, IDictionary<string, IResultsAPI> resultsAPI) : IImportManager
{
	public string[] Sources => resultsAPI.Keys.ToArray();

	public async Task<Athlete[]> ImportAthletes(string source, uint externalID)
	{
		var importer = resultsAPI[source];
		var results = await importer.GetResults(externalID);
		var jsonAthletes = results.GetProperty("Racers").EnumerateArray();
		var startList = await GetStartList(source, externalID);

		var legacyAthletes = jsonAthletes.Select(j => importer.Source.ParseAthlete(j, ReadOnlyDictionary<string, string>.Empty)).ToArray();
		var athletes = new List<Athlete>();

		foreach (var legacyAthlete in legacyAthletes)
		{
			var newInfo = startList.FirstOrDefault(a
				=> (a.TryGetProperty("Name", out var name) && name.GetString() == legacyAthlete.Name)
				   && (a.TryGetProperty("Age", out var age) && age.GetByte() == legacyAthlete.Age)
			);
			var fullLegacyAthlete = newInfo.ValueKind is JsonValueKind.Object
				? legacyAthlete with
				{
					DateOfBirth = legacyAthlete.DateOfBirth ?? newInfo.GetProperty("Info1").GetDateTime(),
					Private = legacyAthlete.Private || newInfo.GetProperty("Info2").GetString() is "Y",
				}
				: legacyAthlete;
			var athlete = await legacyConverter.GetAthlete(source, fullLegacyAthlete);
			athletes.Add(athlete);
		}

		return athletes.ToArray();
	}

	private async Task<JsonElement.ArrayEnumerator> GetStartList(string source, uint externalID)
	{
		var json = source is nameof(WebScorer)
			? await resultsAPI[nameof(WebScorerStartList)].GetResults(externalID)
			: JsonElement.Parse(@"{""StartList"":[]}");
		return json.GetProperty("StartList").EnumerateArray();
	}

	public async Task<Result[]> ImportResults(Course course, string source, uint externalID, DateTime? dateOverride = null)
	{
		var importer = resultsAPI[source];
		var response = await importer.GetResults(externalID);
		var dataSource = importer.Source;
		var legacyResults = dataSource.ParseCourse(null, response, null);
		var results = await legacyConverter.ConvertResults(course.ID, source, legacyResults, dateOverride);

		await resultService.Import(results);
		return results;
	}
}