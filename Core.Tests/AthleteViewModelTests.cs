using System;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests;

public class AthleteViewModelTests
{
	[Fact]
	public void TitleIsAthleteName()
	{
		//arrange
		var vm = new AthleteSummaryViewModel { Summary = new AthleteSummary(new Athlete { Name = "Steve Desmond" }, Array.Empty<Course>()) };

		//act
		var title = vm.Title;

		//assert
		Assert.Equal("Steve Desmond", title);
	}
}