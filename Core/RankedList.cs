namespace FLRC.Leaderboards.Core;

public class RankedList<T> : List<Ranked<T>>
{
	public RankedList()
	{
	}

	public RankedList(IEnumerable<Ranked<T>> list)
	{
		AddRange(list);
	}
}