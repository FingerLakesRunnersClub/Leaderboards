using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Controllers;
using FLRC.Leaderboards.Web.ViewModels;
using NSubstitute;
using Xunit;
using Athlete = FLRC.Leaderboards.Core.Athletes.Athlete;

namespace FLRC.Leaderboards.Web.Tests.Controllers;

public sealed class CompletedControllerTests
{
	[Fact]
	public async Task CanGetListOfCompletions()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var dataService = Substitute.For<IDataService>();

		var controller = new CompletedController(iterationManager, dataService);

		var course = ResultsData.Course with { Results = ResultsData.Results };
		var iteration = new Iteration
		{
			Challenges = [new Challenge { IsOfficial = true, IsPrimary = true, Courses = [course] }],
			Races = [new Race { Courses = [course]}]
		};
		iterationManager.ActiveIteration().Returns(iteration);

		var personal = new Dictionary<Athlete, DateOnly>
		{
			{ CourseData.Athlete1, new DateOnly(2023, 08, 09) },
			{ CourseData.Athlete2, new DateOnly(2023, 08, 09) }
		};
		dataService.GetPersonalCompletions().Returns(personal);

		//act
		var response = await controller.Index();

		//assert
		var vm = response.Model as ViewModel<Completed>;
		Assert.Equal(4, vm!.Data.Results.Count);
		Assert.Equal(2, vm!.Data.PersonalResults.Count);
	}
}