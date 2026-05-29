using System.Collections;

namespace FLRC.Leaderboards.Core.Results;

public abstract record Grouped<TG, TR>(IGrouping<TG, TR> Group) : IGrouping<TG, TR>, IComparable<Grouped<TG, TR>>
{
	public TG Key => Group.Key;
	public IEnumerator<TR> GetEnumerator() => Group.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public virtual int CompareTo(Grouped<TG, TR> other) => GetHashCode().CompareTo(other.GetHashCode());
}