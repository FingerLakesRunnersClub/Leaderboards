using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Ranking;

public sealed class RankedListTests
{
	[Fact]
	public void CanCreateEmptyRankedList()
	{
		//act
		var rankedList = new RankedList<byte>();

		//assert
		Assert.Empty(rankedList);
	}

	[Fact]
	public void CanCreateRankedListFromList()
	{
		//arrange
		var list = new Ranked<byte, Result>[]
		{
			new() { Rank = new Rank(1) },
			new() { Rank = new Rank(2) },
			new() { Rank = new Rank(3) }
		};

		//act
		var rankedList = new RankedList<byte, Result>(list);

		//assert
		Assert.Equal(3, rankedList.Count);
	}
}