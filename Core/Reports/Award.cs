using FLRC.Leaderboards.Core.Athletes;

namespace FLRC.Leaderboards.Core.Reports;

public sealed class Award
{
	public string Name { get; init; }
	public string Link { get; init; }
	public byte Value { get; init; }
	public Athlete Athlete { get; init; }
}