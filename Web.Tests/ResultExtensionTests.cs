using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Model;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests;

public sealed class ResultExtensionTests
{
	[Fact]
	public void CanGetTimeBehindOtherResult()
	{
		//arrange
		var r1 = new Result { Duration = new TimeSpan(1, 2, 3) };
		var r2 = new Result { Duration = new TimeSpan(4, 6, 8) };

		//act
		var behind = r2.Behind(r1);

		//assert
		Assert.Equal(new Time(new TimeSpan(3, 4, 5)), behind);
	}
}