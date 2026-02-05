namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record HeadViewModel
{
	public required string Context { get; init; }
	public required string AppName { get; init; }
	public required string PageTitle { get; init; }
}