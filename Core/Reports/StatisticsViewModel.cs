namespace FLRC.Leaderboards.Core.Reports;

public class StatisticsViewModel : ViewModel
{
	public override string Title => "Statistics";
	public IDictionary<Course, Statistics> Courses { get; init; }
	public IDictionary<DateTime, Statistics> History { get; init; }
	public Statistics Total { get; init; }
}