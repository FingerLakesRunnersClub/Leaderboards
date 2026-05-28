using System.Collections;

namespace FLRC.Leaderboards.Core.Results;

public abstract class Grouped<TG, TR>(IGrouping<TG, TR> group) : IGrouping<TG, TR>, IComparable<Grouped<TG, TR>>
{
	public TG Key => group.Key;
	public IEnumerator<TR> GetEnumerator() => group.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public virtual int CompareTo(Grouped<TG, TR> other) => GetHashCode().CompareTo(other.GetHashCode());
}