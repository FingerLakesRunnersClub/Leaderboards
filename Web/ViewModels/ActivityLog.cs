using System.Collections.Immutable;
using FLRC.Leaderboards.Core.Reports;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record ActivityLog
{
	public ActivityLogType Type { get; init; }
	public Course Course { get; init; }
	public IGrouping<string, Result>[] Results { get; init; }
	public string CourseLabel => "Course";

	public static readonly IImmutableDictionary<ActivityLogType, string> Types = new Dictionary<ActivityLogType, string>
	{
		{ActivityLogType.Recent, "Index"},
		{ActivityLogType.Archive, "All"}
	}.ToImmutableDictionary();
}