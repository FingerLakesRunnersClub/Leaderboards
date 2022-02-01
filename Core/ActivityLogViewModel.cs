using System.Collections.Immutable;

namespace FLRC.Leaderboards.Core;

public class ActivityLogViewModel : ViewModel
{
	public override string Title => $"Activity Log" + (Course != null ? $" — {Course.Name}" : "");

	public ActivityLogType Type { get; init; }
	public Course Course { get; init; }
	public IEnumerable<IGrouping<string, Result>> Results { get; init; }

	public static readonly IImmutableDictionary<ActivityLogType, string> LogTypes = new Dictionary<ActivityLogType, string>
		{
			{ActivityLogType.Recent, "Index"},
			{ActivityLogType.Archive, "All"},
		}.ToImmutableDictionary();
}