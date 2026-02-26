using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Data.Models;

namespace FLRC.Leaderboards.Web.Areas.Admin.Services;

public sealed class RegistrationManager(IImportManager importManager, IIterationService iterationService, IResultService resultService) : IRegistrationManager
{
	public async Task<Athlete[]> Update(Iteration iteration)
	{
		var athletes = await GetAthletes(iteration);
		await iterationService.UpdateRegistrations(iteration, athletes);
		return athletes;
	}

	private async Task<Athlete[]> GetAthletes(Iteration iteration)
	{
		switch (iteration.RegistrationType)
		{
			case nameof(WebScorer):
				var externalID = uint.Parse(iteration.RegistrationContext ?? throw new ArgumentException("Registration context not provided"));
				return await importManager.ImportAthletes(iteration.RegistrationType, externalID);
			case "AnyRace":
				return await AllRaceParticipants(iteration);
			default:
				throw new NotImplementedException($"No registration manager for '{iteration.RegistrationType}'");
		}
	}

	private async Task<Athlete[]> AllRaceParticipants(Iteration iteration)
	{
		var courses = iteration.Races.SelectMany(r => r.Courses);

		var results = new List<Result>();
		foreach (var course in courses)
			results.AddRange(await resultService.Find(course.ID));

		var start = iteration.StartDate.HasValue ? new DateTime(iteration.StartDate.Value, TimeOnly.MinValue) : (DateTime?)null;
		var end = iteration.EndDate.HasValue ? new DateTime(iteration.EndDate.Value, TimeOnly.MaxValue) : (DateTime?)null;
		return results.Where(r => r.StartTime >= start && r.StartTime <= end).Select(r => r.Athlete).Distinct().ToArray();
	}
}