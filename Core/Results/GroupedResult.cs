using System.Collections;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;

namespace FLRC.Leaderboards.Core.Results;

public sealed class GroupedResult : IGrouping<Athlete, Result>, IComparable<GroupedResult>
{
	private readonly IGrouping<Athlete, Result> _group;

	public GroupedResult(IGrouping<Athlete, Result> group) => _group = group;

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

	public Athlete Key => _group.Key;
	public IEnumerator<Result> GetEnumerator() => _group.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public int CompareTo(GroupedResult other) => _group.Key.ID.CompareTo(other.Key.ID);
}