using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Teams;

namespace FLRC.Leaderboards.Core.Results;

public sealed record Filter(Category Category = null, Team AgeGroup = null)
{
	public static readonly Filter None = new();
	public static readonly Filter F = new(Category.F);
	public static readonly Filter M = new(Category.M);

	public bool IsMatch(Result result)
		=> (Category == null || result.Athlete.Category == Category)
		   && (AgeGroup == null || (result.AgeOnDayOfRun >= AgeGroup.MinAge && (AgeGroup.MaxAge == null || result.AgeOnDayOfRun <= AgeGroup.MaxAge)));
}