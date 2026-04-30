using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record Completed
{
	public RankedList<Date, Result> Results { get; init; }
	public RankedList<Date, Result> PersonalResults { get; init; }
}