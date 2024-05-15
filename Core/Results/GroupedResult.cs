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
		=> new()
		{
			Athlete = Key,
			Course = course,
			Duration = !Key.Private
				? new Time(TimeSpan.FromSeconds(_group.OrderBy(r => r.Duration)
					.Take(threshold ?? _group.Count()).Average(r => r.Duration.Value.TotalSeconds)))
				: null
		};
}