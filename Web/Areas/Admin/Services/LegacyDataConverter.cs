using FLRC.Leaderboards.Data.Models;

namespace FLRC.Leaderboards.Web.Areas.Admin.Services;

public sealed class LegacyDataConverter(IAthleteService athleteService) : ILegacyDataConverter
{
	public async Task<Result[]> ConvertResults(Guid courseID, string source, Core.Results.Result[] legacyResults, DateTime? dateOverride = null)
	{
		var results = new List<Result>();
		foreach (var legacyResult in legacyResults.Where(r => r.Duration is not null))
		{
			var result = await ConvertResult(courseID, source, legacyResult, dateOverride);
			results.Add(result);
		}

		return results.ToArray();
	}

	private async Task<Result> ConvertResult(Guid courseID, string source, Core.Results.Result result, DateTime? dateOverride)
	{
		var athlete = await GetAthlete(source, result.Athlete);
		return new Result
		{
			ID = Guid.NewGuid(),
			AthleteID = athlete.ID,
			CourseID = courseID,
			StartTime = (dateOverride ?? result.StartTime.Value).ToUniversalTime(),
			Duration = result.Duration.Value
		};
	}

	public async Task<Athlete> GetAthlete(string source, Core.Athletes.Athlete legacyAthlete)
	{
		var athlete = await athleteService.Find(source, legacyAthlete.ID.ToString());
		if (athlete is null && legacyAthlete.DateOfBirth.HasValue)
			athlete = await athleteService.Find(legacyAthlete.Name, DateOnly.FromDateTime(legacyAthlete.DateOfBirth.Value));

		var newAthlete = ConvertLegacyAthlete(source, legacyAthlete);
		var newLink = newAthlete.LinkedAccounts.Single();
		if (athlete is null)
		{
			athlete = newAthlete;
			await athleteService.AddAthlete(athlete);
		}

		if ((athlete.DateOfBirth == Athlete.UnknownDOB && newAthlete.DateOfBirth != Athlete.UnknownDOB)
		    || (athlete.Category == Athlete.UnknownCategory && newAthlete.Category != Athlete.UnknownCategory)
		    || !athlete.LinkedAccounts.Any(a => a.Type == newLink.Type && a.Value == newLink.Value))
			await athleteService.UpdateAthlete(athlete, newAthlete);

		return athlete;
	}

	private static Athlete ConvertLegacyAthlete(string source, Core.Athletes.Athlete athlete)
		=> new()
		{
			ID = Guid.NewGuid(),
			Name = athlete.Name,
			DateOfBirth = athlete.DateOfBirth.HasValue ? DateOnly.FromDateTime(athlete.DateOfBirth.Value) : Athlete.UnknownDOB,
			Category = athlete.Category?.Display[0] ?? Athlete.UnknownCategory,
			IsPrivate = athlete.Private,
			LinkedAccounts = [new LinkedAccount { ID = Guid.NewGuid(), Type = source, Value = athlete.ID.ToString() }]
		};
}