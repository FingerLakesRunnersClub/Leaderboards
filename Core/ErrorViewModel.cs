namespace FLRC.Leaderboards.Core;

public class ErrorViewModel : ViewModel
{
	public override string Title => Error != null ? "Error" : "Not Found";

	public Exception Error { get; init; }
}