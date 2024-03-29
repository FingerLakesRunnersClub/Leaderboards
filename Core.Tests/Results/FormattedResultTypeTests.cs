using FLRC.Leaderboards.Core.Results;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Results;

public sealed class FormattedResultTypeTests
{
	[Fact]
	public void CanDisplayName()
	{
		//arrange
		var type = new FormattedResultType(ResultType.BestAverage);

		//act
		var name = type.Display;

		//assert
		Assert.Equal("Best Average", name);
	}
}