using System.Collections;
using FLRC.Leaderboards.Core.Athletes;

namespace FLRC.Leaderboards.Core.Results;

public class Grouped<T> : IGrouping<Athlete, T>, IComparable<Grouped<T>>
{
	protected readonly IGrouping<Athlete, T> _group;

	public Grouped(IGrouping<Athlete, T> group) => _group = group;


	public Athlete Key => _group.Key;
	public IEnumerator<T> GetEnumerator() => _group.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public int CompareTo(Grouped<T> other) => _group.Key.ID.CompareTo(other.Key.ID);
}