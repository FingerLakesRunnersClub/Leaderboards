using FLRC.Leaderboards.Core.Athletes;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Athletes;

public sealed class NameExtensionsTests
{
	[Fact]
	public void IdenticalNamesProduceIdenticalIDs()
	{
		//arrange
		const string name1 = "Steve Desmond";
		const string name2 = "Steve Desmond";

		//act
		var id1 = name1.GetID();
		var id2 = name2.GetID();

		//assert
		Assert.Equal(id1, id2);
	}
}