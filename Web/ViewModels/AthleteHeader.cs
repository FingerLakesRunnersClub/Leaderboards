using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record AthleteHeader
{
	public Athlete Athlete { get; init; }
	public IDictionary<string, string> Badges { get; init; }
}