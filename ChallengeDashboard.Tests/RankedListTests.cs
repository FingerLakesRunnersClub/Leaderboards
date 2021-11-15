using System.Collections.Generic;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests;

public class RankedListTests
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
		var list = new List<Ranked<byte>>
			{
				new () { Rank = new (1) },
				new () { Rank = new (2) },
				new () { Rank = new (3) }
			};

		//act
		var rankedList = new RankedList<byte>(list);

		//assert
		Assert.Equal(3, rankedList.Count);
	}
}
