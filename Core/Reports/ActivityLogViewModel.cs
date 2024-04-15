using System.Collections.Immutable;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Reports;

public sealed class ActivityLogViewModel : ViewModel
{
	public override string Title => "Activity Log" + (Course != null ? $" — {Course.Name}" : string.Empty);

	public ActivityLogType Type { get; init; }
	public Course Course { get; init; }
	public IReadOnlyCollection<IGrouping<string, Result>> Results { get; init; }

	public static readonly IImmutableDictionary<ActivityLogType, string> LogTypes = new Dictionary<ActivityLogType, string>
		{
			{ActivityLogType.Recent, "Index"},
			{ActivityLogType.Archive, "All"}
		}.ToImmutableDictionary();
}