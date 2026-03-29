using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.Areas.Admin.ViewModels;

public sealed record AthleteForm
{
	public Athlete Athlete { get; init; }
	public bool IsAdmin { get; init; }
}