using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Teams;

namespace FLRC.Leaderboards.Core.Results;

public record Filter(Category Category = null, Team AgeGroup = null, byte? Month = null)
{
	public static readonly Filter F = new(Category.F);
	public static readonly Filter M = new(Category.M);

	public bool IsMatch(Result result)
		=> (Category == null || result.Athlete.Category == Category)
		   && (Month == null || result.StartTime.Value.Month == Month)
		   && (AgeGroup == null || (result.AgeOnDayOfRun >= AgeGroup.MinAge && (AgeGroup.MaxAge == null || result.AgeOnDayOfRun <= AgeGroup.MaxAge)));
}