using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Athletes;

public sealed class AthleteOverallRow : Ranked<string, Result>
{
	public string ID { get; init; }
	public string Name { get; init; }
}