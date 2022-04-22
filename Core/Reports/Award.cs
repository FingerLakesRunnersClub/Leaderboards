using FLRC.Leaderboards.Core.Athletes;

namespace FLRC.Leaderboards.Core.Reports;

public class Award
{
	public string Name { get; init; }
	public byte Value { get; init; }
	public Athlete Athlete { get; init; }
}