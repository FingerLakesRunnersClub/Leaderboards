using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Components;
using FLRC.Leaderboards.Web.Services;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Components;

public sealed class AthleteHeaderTests
{
	[Fact]
	public async Task CanShowHeaderWithAthleteName()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var overall = Substitute.For<IOverallResultsCalculator>();
		var component = new AthleteHeader(iterationManager, overall);

		overall.Completed(Arg.Any<Iteration>()).Returns([]);

		//act
		var athlete = new Athlete { Name = "Test 123" };
		var result = await component.InvokeAsync(athlete);

		//assert
		Assert.IsType<ViewViewComponentResult>(result);
	}
}