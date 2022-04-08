using FLRC.Leaderboards.Core.Ranking;

namespace FLRC.Leaderboards.Core.Athletes;

public class AthleteOverallRow : Ranked<string>
{
	public string ID { get; init; }
	public string Name { get; init; }
}