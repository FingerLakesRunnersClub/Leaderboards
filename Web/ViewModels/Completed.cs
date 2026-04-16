using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Model;
using Athlete = FLRC.Leaderboards.Core.Athletes.Athlete;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record Completed
{
	public RankedList<Date, Result> Results { get; init; }
	public IDictionary<Athlete, DateOnly> PersonalResults { get; init; }
}