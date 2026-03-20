using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Ranking;

public sealed class RankedList<T> : List<Ranked<T, Result>>;

public sealed class RankedList<T,R> : List<Ranked<T,R>>
{
	public RankedList()
	{
	}

	public RankedList(Ranked<T,R>[] list)
	{
		AddRange(list);
	}
}