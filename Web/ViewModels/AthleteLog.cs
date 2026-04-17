using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record AthleteLog
{
	public Athlete User { get; init; }
	public bool Admin { get; init; }
	public Athlete Athlete { get; init; }
	public RankedList<Time, Result> Results { get; init; }
}