using FLRC.Leaderboards.Core.Races;

namespace FLRC.Leaderboards.Core.Reports;

public sealed class StatisticsViewModel : ViewModel
{
	public override string Title => "Statistics";
	public IDictionary<Course, Statistics> Courses { get; init; }
	public IDictionary<DateOnly, Statistics> History { get; init; }
	public Statistics Total { get; init; }
}