using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.Areas.Admin.ViewModels;

public sealed class MergeAthletesForm
{
	public Athlete Current { get; init; }
	public Athlete[] Athletes { get; init; }
}