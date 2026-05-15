using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed class Award
{
	public string Name { get; init; }
	public string Link { get; init; }
	public byte Value { get; init; }
	public Athlete Athlete { get; init; }
}