using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web;

public sealed record Filter(Category Category = null, Team AgeGroup = null, Iteration Iteration = null)
{
	public static readonly Filter None = new();

	public bool IsMatch(Result result)
		=> (Category == null || result.Athlete.Category == Category.Display[0])
		   && (AgeGroup == null || (result.AthleteAge >= AgeGroup.MinAge && (AgeGroup.MaxAge == null || result.AthleteAge <= AgeGroup.MaxAge)))
		   && (Iteration == null || (DateOnly.FromDateTime(result.StartTime) >= Iteration.StartDate && DateOnly.FromDateTime(result.StartTime.Add(result.Duration)) <= Iteration.EndDate));

};