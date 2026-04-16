using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed class SimilarAthletes
{
	public Athlete Athlete { get; init; }
	public SimilarAthlete[] Matches { get; init; }
}