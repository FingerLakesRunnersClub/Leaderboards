using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;

namespace FLRC.Leaderboards.Core.Results;

public sealed class GroupedResult : Grouped<Result>
{
	public GroupedResult(IGrouping<Athlete, Result> group) : base(group)
	{
	}

	public Result Average(Course course, ushort? threshold = null)
	{
		var timedResults = _group.Where(r => r.Duration is not null).ToArray();
		var average = timedResults.Length > 0
			? timedResults.OrderBy(r => r.Duration)
				.Take(threshold ?? timedResults.Length)
				.Average(r => r.Duration.Value.TotalSeconds)
			: 0;

		return new Result
		{
			Athlete = Key,
			Course = course,
			Duration = !Key.Private && timedResults.Length > 0
				? new Time(TimeSpan.FromSeconds(average))
				: null
		};
	}
}