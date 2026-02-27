using FLRC.Leaderboards.Model;
using Xunit;

namespace FLRC.Leaderboards.Services.Tests;

public sealed class LinkedAccountComparerTests
{
	[Fact]
	public void AccountsMatchWhenTypeAndValueMatch()
	{
		//arrange
		var comparer = new LinkedAccountComparer();

		var a1 = new LinkedAccount { Type = "t1", Value = "v1" };
		var a2 = new LinkedAccount { Type = "t1", Value = "v1" };

		//act
		var equal = comparer.Equals(a1, a2);

		//assert
		Assert.True(equal);
	}

	[Fact]
	public void AccountsDoNotMatchWhenValueDoesNotMatch()
	{
		//arrange
		var comparer = new LinkedAccountComparer();

		var a1 = new LinkedAccount { Type = "t1", Value = "v1" };
		var a2 = new LinkedAccount { Type = "t1", Value = "v2" };

		//act
		var equal = comparer.Equals(a1, a2);

		//assert
		Assert.False(equal);
	}

	[Fact]
	public void AccountsDoNotMatchWhenTypeDoesNotMatch()
	{
		//arrange
		var comparer = new LinkedAccountComparer();

		var a1 = new LinkedAccount { Type = "t1", Value = "v1" };
		var a2 = new LinkedAccount { Type = "t2", Value = "v1" };

		//act
		var equal = comparer.Equals(a1, a2);

		//assert
		Assert.False(equal);
	}
}