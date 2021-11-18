using Xunit;

namespace FLRC.ChallengeDashboard.Tests;

public class FormattedResultTypeTests
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