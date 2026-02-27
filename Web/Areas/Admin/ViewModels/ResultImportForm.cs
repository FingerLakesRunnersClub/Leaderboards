using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.Areas.Admin.ViewModels;

public sealed record ResultImportForm
{
	public Race[] Races { get; init; }
	public string[] Sources { get; set; }
}