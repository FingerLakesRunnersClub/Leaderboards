namespace FLRC.Leaderboards.Core.Ranking;

public sealed class RankedList<T,TR> : List<Ranked<T,TR>>
{
	public RankedList()
	{
	}

	public RankedList(Ranked<T,TR>[] list)
	{
		AddRange(list);
	}
}