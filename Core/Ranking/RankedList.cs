namespace FLRC.Leaderboards.Core.Ranking;

public sealed class RankedList<T> : List<Ranked<T>>
{
	public RankedList()
	{
	}

	public RankedList(IEnumerable<Ranked<T>> list)
	{
		AddRange(list);
	}
}