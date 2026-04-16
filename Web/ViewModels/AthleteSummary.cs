using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Model;
using Athlete = FLRC.Leaderboards.Model.Athlete;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record AthleteSummary
{
	public Iteration Iteration { get; init; }
	public Result[] AllResults { get; init; }
	public Athlete Athlete { get; init; }
	public Dictionary<Course, Ranked<Time, Result>> Fastest { get; init; }
	public Dictionary<Course, Ranked<Performance, Result>> Farthest { get; init; }
	public Dictionary<Course, Ranked<Time, Result>> Average { get; init; }
	public Dictionary<Course, Ranked<ushort, Result>> Runs { get; init; }
	public Dictionary<Course, Ranked<Stars, Result>> CommunityStars { get; init; }
	public Dictionary<Course, Result[]> All { get; init; }

	public AthleteOverallRow[] Competitions { get; init; }

	public Ranked<Points, Result> OverallPoints { get; init; }
	public Ranked<AgeGrade, Result> OverallAgeGrade { get; init; }
	public Ranked<Miles, Result> OverallMiles { get; init; }
	public Ranked<Stars, Result> OverallCommunityStars { get; init; }
	public Ranked<TeamResults, Result> TeamResults { get; init; }

	public int TotalResults { get; init; }
}