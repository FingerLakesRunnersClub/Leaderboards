namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record ViewModel<T>(string Title, T Data)
{
	public string Error { get; init; }
}