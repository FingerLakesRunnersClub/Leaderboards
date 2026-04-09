using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record ChallengeResult
{
	public Challenge Challenge { get; init; }
	public Athlete Athlete { get; init; }
	public Result[] Results { get; init; }
	public Date StartTime { get; init; }
	public Date FinishTime { get; init; }
	public Time RunningTime { get; init; }
	public Time TotalTime { get; init; }

	public byte? AthleteAge => Results.First().AthleteAge;
}