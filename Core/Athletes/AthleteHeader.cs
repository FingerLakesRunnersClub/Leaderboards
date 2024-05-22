namespace FLRC.Leaderboards.Core.Athletes;

public sealed class AthleteHeader
{
	public Athlete Athlete { get; init; }
	public IDictionary<string, string> BadgeIcons { get; init; }
	public bool ShowLink { get; init; }
}