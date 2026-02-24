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

			if (!results.Any(r => r.AthleteID == result.AthleteID && r.StartTime == result.StartTime && r.Duration == result.Duration))
				results.Add(result);
		}

		return results.ToArray();
	}

	private async Task<Result> ConvertResult(Guid courseID, string source, Core.Results.Result result, DateTime? dateOverride)
	{
		var date = (dateOverride ?? result.StartTime.Value).ToUniversalTime();
		var athlete = await GetAthlete(source, result.Athlete, date);
		return new Result
		{
			ID = Guid.NewGuid(),
			AthleteID = athlete.ID,
			CourseID = courseID,
			StartTime = date,
			Duration = result.Duration.Value
		};
	}

	public async Task<Athlete> GetAthlete(string source, Core.Athletes.Athlete legacyAthlete, DateTime? date = null)
	{
		var athlete = await athleteService.Find(source, legacyAthlete.ID.ToString());

		if (athlete is null && legacyAthlete.DateOfBirth.HasValue)
			athlete = await athleteService.Find(legacyAthlete.Name, DateOnly.FromDateTime(legacyAthlete.DateOfBirth.Value));

		if (athlete is null && date.HasValue && legacyAthlete.Age > 0)
			athlete = await athleteService.Find(legacyAthlete.Name, legacyAthlete.Age, date.Value);

		var newAthlete = ConvertLegacyAthlete(source, legacyAthlete);
		if (athlete is null)
		{
			athlete = newAthlete;
			await athleteService.AddAthlete(athlete);
		}

		if ((athlete.DateOfBirth == Athlete.UnknownDOB && newAthlete.DateOfBirth != Athlete.UnknownDOB)
		    || (athlete.Category == Athlete.UnknownCategory && newAthlete.Category != Athlete.UnknownCategory)
		    || newAthlete.LinkedAccounts.Any(account => !athlete.LinkedAccounts.Contains(account, AthleteService.LinkedAccountComparer)))
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
			LinkedAccounts = new[] {
				new LinkedAccount { ID = Guid.NewGuid(), Type = source, Value = athlete.ID.ToString() },
				new LinkedAccount { ID = Guid.NewGuid(), Type = "Email", Value = athlete.Email }
			}.Where(a => !string.IsNullOrWhiteSpace(a.Value)).ToArray()
		};
}