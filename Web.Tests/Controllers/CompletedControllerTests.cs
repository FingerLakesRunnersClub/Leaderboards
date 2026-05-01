using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Controllers;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Controllers;

public sealed class CompletedControllerTests
{
	[Fact]
	public async Task CanGetListOfCompletions()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var starCalculator = Substitute.For<ICommunityStarCalculator>();
		var controller = new CompletedController(iterationManager, starCalculator);

		var course = ResultsData.Course with { Results = ResultsData.Results };
		var iteration = new Iteration
		{
			Challenges = [new Challenge { IsOfficial = true, IsPrimary = true, Courses = [course] }],
			Races = [new Race { Courses = [course] }]
		};
		iterationManager.ActiveIteration().Returns(iteration);

		//act
		var response = await controller.Index();

		//assert
		var vm = response.Model as ViewModel<Completed>;
		Assert.Equal(4, vm!.Data.Results.Count);
	}
}