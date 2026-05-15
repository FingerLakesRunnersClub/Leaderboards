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
		var overall = Substitute.For<IOverallResultsCalculator>();
		var controller = new CompletedController(iterationManager, overall);

		var course = ResultsData.Course with { Results = ResultsData.Results };
		var iteration = new Iteration { Races = [new Race { Courses = [course] }] };
		var challenge = new Challenge { IsOfficial = true, IsPrimary = true, Courses = [course], Iteration = iteration };
		iteration.Challenges.Add(challenge);
		var athletes = iteration.Races.SelectMany(r => r.Courses).SelectMany(c => c.Results).Select(r => r.Athlete).Distinct();
		foreach (var athlete in athletes)
			athlete.Challenges.Add(challenge);

		iterationManager.ActiveIteration().Returns(iteration);
		var completed = new OverallResultsCalculator(Substitute.For<ICommunityStarCalculator>()).Completed(iteration);
		overall.Completed(iteration).Returns(completed);

		//act
		var response = await controller.Index();

		//assert
		var vm = response.Model as ViewModel<Completed>;
		Assert.Equal(4, vm!.Data.Results.Count);
	}
}