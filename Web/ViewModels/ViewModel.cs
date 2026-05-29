namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record ViewModel<T>(string Title, T Data) : ViewModel;

public abstract record ViewModel
{
	public abstract string Title { get; init; }
	public string Error { get; init; }
}