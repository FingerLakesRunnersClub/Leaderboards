using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Services;

namespace FLRC.Leaderboards.Importer;

public sealed class Importer(IImportManager importManager, IRaceService raceService, Action<string> log)
{
	public async Task Run(CourseImportConfig[] definitions)
	{
		log("Getting all courses...");
		var races = await raceService.GetAllRaces();
		var courses = races.SelectMany(r => r.Courses).ToArray();
		log($"{courses.Length} courses found!");

		foreach (var definition in definitions)
		{
			var course = courses.FirstOrDefault(c => c.Race.Name == definition.Name
			                                         && (definition.Distance is null
			                                             || new Distance(c.DistanceDisplay).Equals(new Distance(definition.Distance))
			                                             || (c.DistanceDisplay == "13.1 mi" && definition.Distance == "Half Marathon")
			                                             || (c.DistanceDisplay == "26.2 mi" && definition.Distance == "Marathon")));
			switch (course)
			{
				case null when definition.Name.EndsWith("Participants"):
					await importManager.ImportAthletes(definition.Type, definition.ExternalID);
					break;
				case null:
					log($"Course not found for {definition.Name}!");
					continue;
				default:
					await importManager.ImportResults(course, definition.Type, definition.ExternalID, definition.Date);
					break;
			}

			log($"Imported \"{definition.Name} ({course?.DistanceDisplay})\"!");
		}
	}
}