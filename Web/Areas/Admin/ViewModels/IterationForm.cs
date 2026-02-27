using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.Areas.Admin.ViewModels;

public sealed record IterationForm
{
	public required Iteration Iteration { get; init; }
	public required Race[] Races { get; init; }
}