using FLRC.Leaderboards.Core.Reports;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record StatisticsViewModel
{
	public IDictionary<Course, Statistics> Courses { get; init; }
	public IDictionary<DateOnly, Statistics> History { get; init; }
	public Statistics Total { get; init; }
}