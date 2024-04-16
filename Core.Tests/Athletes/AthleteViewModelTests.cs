using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Races;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Athletes;

public sealed class AthleteViewModelTests
{
	[Fact]
	public void TitleIsAthleteName()
	{
		//arrange
		var vm = new AthleteSummaryViewModel { Summary = new AthleteSummary(new Athlete { Name = "Steve Desmond" }, [], TestHelpers.Config) };

		//act
		var title = vm.Title;

		//assert
		Assert.Equal("Steve Desmond", title);
	}
}