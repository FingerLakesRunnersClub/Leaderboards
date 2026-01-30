using FLRC.Leaderboards.Data.Models;

namespace FLRC.Leaderboards.Web.Areas.Admin.ViewModels;

public sealed record IterationForm
{
	public required Iteration Iteration { get; init; }
	public required Race[] Races { get; init; }
}