using System.Collections;
using FLRC.Leaderboards.Core.Athletes;

namespace FLRC.Leaderboards.Core.Results;

public abstract class Grouped<T> : Grouped<Athlete, T>
{
	protected Grouped(IGrouping<Athlete, T> group) : base(group)
	{
	}

	public int CompareTo(Grouped<T> other) => _group.Key.ID.CompareTo(other.Key.ID);
}

public abstract class Grouped<G, T> : IGrouping<G, T>, IComparable<Grouped<G,T>>
{
	protected readonly IGrouping<G, T> _group;

	public Grouped(IGrouping<G, T> group) => _group = group;


	public G Key => _group.Key;
	public IEnumerator<T> GetEnumerator() => _group.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public virtual int CompareTo(Grouped<G, T> other) => GetHashCode().CompareTo(other.GetHashCode());
}